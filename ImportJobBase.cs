using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SC = Sitecore;
using SCData = Sitecore.Data;
using SCItems = Sitecore.Data.Items;
using SCFields = Sitecore.Data.Fields;

using Specialized.Content.Import.Interfaces;
using Specialized.Content.Import.SupportClasses;
using Specialized.Content.Import.Mapping.Xls;
using Specialized.Content.Import.Mapping;

namespace Specialized.Content.Import
{
    public class ImportJobBase : IHasImportStatus<StatusMessage>
    {
        #region Public properties

        /// <summary>
        /// Returns the name of the CurrentMapping being imported during a job
        /// </summary>
        public string CurrentMappingName { get; protected set; }

        /// <summary>
        /// Returns the number of the current item within the CurrentMapping being imported during a job
        /// </summary>
        public int CurrentItemIndex { get { return this._currentItemIndex; } }

        public bool IsAsync { get; set; } = false;

        /// <summary>
        /// Report whether job is currently running
        /// </summary>
        public bool IsJobRunning { get { return this.ImportStatus == Enumerations.JobStatus.Running; } }

        /// <summary>
        /// Return whether the last running job was canceled
        /// </summary>
        public bool IsJobCanceled { get { return this.ImportStatus == Enumerations.JobStatus.Canceled; } }

        /// <summary>
        /// Returns current status of job
        /// </summary>
        public Enumerations.JobStatus ImportStatus { get; set; } = Enumerations.JobStatus.Idle;

        public List<StatusMessage> ImportMessages { get; set; } = new List<StatusMessage>();

        public Enumerations.ImportJobType ImportType { get; set; }

        public BackgroundWorker BgWorker { get { return this._bgWorker; } }

        /// <summary>
        /// Occurs when progress is updated when job runs asynchronously
        /// </summary>
        public event ProgressChangedHandler OnProgressChange;

        /// <summary>
        /// Occurs when job ends when run asynchronously
        /// </summary>
        public event ProgressEndedHandler OnProgressEnd;

        public event DoWorkEventHandler DoWorkEvent;

        /// <summary>
        /// Returns the percent of rows completed in the CurrentMapping during a job
        /// </summary>
        public int CurrentPercent
        {
            get { return this._currentPercent; }
            protected set { this._currentPercent = value; }
        }

        // event handlers for background worker/async operation
        public delegate void ProgressChangedHandler(object sender, ImportProgressChangedEventArgs e);
        public delegate void ProgressEndedHandler(object sender, ImportProgressEndedEventArgs e);

        #endregion

        #region Private properties

        private readonly BackgroundWorker _bgWorker;
        private int _currentPercent = 0;

        #endregion

        #region Protected properties
        protected int _currentItemIndex = -1;
        protected SCData.Database _database = null;
        protected SCItems.Item _parent = null;
        #endregion

        #region Public methods
        public ImportJobBase()
        {
            this._bgWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this._bgWorker.ProgressChanged += bgWorkerProgressChanged;
            this._bgWorker.RunWorkerCompleted += bgWorkerProgressEnded;
        }

        /// <summary>
        /// Start importing data from ImportMappings using BackgroundWorker
        /// </summary>
        public void ImportDataAsync()
        {
            this._bgWorker.DoWork += this.DoWorkEvent;
            this._bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Start importing data from ImportMappings
        /// </summary>
        public void ImportData()
        {
            this.DoWorkEvent(null, null);
        }

        /// <summary>
        /// Allows cancellation of import job
        /// </summary>
        public void CancelImport()
        {
            if (this.IsAsync)
            {
                this._bgWorker.CancelAsync();
            }
            else
            {
                this.ImportStatus = Enumerations.JobStatus.Canceled;
            }
        }

        /// <summary>
        /// Static constructor to load assembly
        /// </summary>
        static ImportJobBase()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String dllName = new AssemblyName(args.Name).Name + ".dll";
                var execAss = Assembly.GetExecutingAssembly();
                string resName = execAss.GetManifestResourceNames().FirstOrDefault(mrn => mrn.EndsWith(dllName));
                if (resName == null)
                {
                    return null;
                }
                using (var stream = execAss.GetManifestResourceStream(resName))
                {
                    byte[] assData = new byte[stream.Length];
                    stream.Read(assData, 0, assData.Length);
                    return Assembly.Load(assData);
                }
            };
        }
        #endregion

        #region Protected methods

        /// <summary>
        /// Get Sitecore Item using sitecore DataBase for current instance
        /// </summary>
        /// <param name="itemID">Item ID as Guide</param>
        /// <returns>Sitecore Item</returns>
        protected SCItems.Item GetItem(Guid itemID)
        {
            return this.GetItem(new SCData.ID(itemID));
        }

        protected SCItems.Item GetItem(SCData.ID itemId)
        {
            return this._database.GetItem(itemId);
        }

        protected void SetParent(MappingBase currentMapping)
        {
            if (currentMapping.ParentIDSource == Enumerations.ParentSource.Global)
            {
                this._parent = this.GetItem(currentMapping.GlobalParentID);

                if (this._parent == null)
                {
                    LogStatus("A valid Item ID must be specified for GlobalParentID when ParentSource is Global.");
                    return;
                }
            }
        }

        protected void InitializeImportBase()
        {
            this.ImportStatus = Enumerations.JobStatus.Running;
            this._database = SC.Configuration.Factory.GetDatabase("master");    
            this.ImportMessages = new List<StatusMessage>();
            if (this._database == null)
            {
                throw new NullReferenceException("Master DB cannot be accessed. Please make sure the import is running with access to the Master DB.");
            }
        }

        /// <summary>
        /// Log an error message to job error list. CurrentItem will be -1 if not due to row or file specific error.
        /// </summary>
        /// <param name="statusMessage">Status message to log</param>
        protected void LogStatus(string statusMessage)
        {
            LogStatus(statusMessage, this._currentItemIndex.ToString());
        }

        /// <summary>
        /// Log an error message allowing an override of the currentItem to specify a file name when appropriate
        /// </summary>
        /// <param name="statusMessage">Status message to log</param>
        /// <param name="currentItem">The item string to log the message related to</param>
        protected void LogStatus(string statusMessage, string currentItem)
        {
            this.ImportMessages.Add(new StatusMessage()
            {
                JobType = this.ImportType,
                MappingName = this.CurrentMappingName,
                CurrentItem = currentItem,
                Message = statusMessage
            });
        }
        #endregion

        #region Background process for Async operation

        /// <summary>
        /// Report progress changes to event delegate supplied by OnProgressChange
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">ProgressChangedEventArgs</param>
        private void bgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this._currentPercent = e.ProgressPercentage;
            ImportProgressChangedEventArgs args = new ImportProgressChangedEventArgs(e.ProgressPercentage, (string)e.UserState, this.IsJobCanceled);
            this.OnProgressChange(this, args);
        }

        /// <summary>
        /// Report when background job ends to delegate supplied by OnProfressEnd
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">RunWorkerCompletedEventArgs</param>
        private void bgWorkerProgressEnded(object sender, RunWorkerCompletedEventArgs e)
        {
            ImportProgressEndedEventArgs args = (ImportProgressEndedEventArgs)e;
            args.PercentCompleted = this._currentPercent;
            this.OnProgressEnd(this, args);
        }

        /// <summary>
        /// Determine whether operation should continue for async & non-canceled, or non-async operations
        /// </summary>
        /// <returns></returns>
        protected bool checkContinue()
        {
            return ((this.IsAsync && !this._bgWorker.CancellationPending) || (!this.IsAsync && !this.IsJobCanceled));
        }

        #endregion
    }
}
