using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;


using SC = Sitecore;
using SCData = Sitecore.Data;
using SCItems = Sitecore.Data.Items;
using SCFields = Sitecore.Data.Fields;

using Specialized.Content.Import.Interfaces;
using Specialized.Content.Import.Mapping.Xml;
using Specialized.Content.Import.SupportClasses;
using Specialized.Content;
using System.ComponentModel;

namespace Specialized.Content.Import
{
    public class XmlImportJob : ImportJobBase, IHasImportStatus<StatusMessage>, IImportJob
    {
        #region Public properties
        //public new List<StatusMessage> ImportMessages { get; set; } = new List<StatusMessage>();

        /// <summary>
        /// Gets list of ImportMappings defined for job
        /// </summary>
        public List<XmlMapping> ImportMappings { get; private set; }

        /// <summary>
        /// Gets or sets the list of MappingSources available for job
        /// </summary>
        public XmlMappingSourceList MappingSources { get; set; }

        #endregion

        #region Private properties

        /// <summary>
        /// List of files for current source directory.
        /// </summary>
        private FileInfo[] currentFileList;
        /// <summary>
        /// List of successfully created items, with Filename source as Key.
        /// </summary>
        private Dictionary<string, SCData.ID> fileToItemList = new Dictionary<string, SCData.ID>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public methods

        public XmlMapping AddMapping(string sourcePath, bool includeAttributeData = false)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException("Please enter a valid path for the source directory.", nameof(sourcePath));
            }
            var sourceDirectory = new DirectoryInfo(sourcePath);
            return AddMapping(sourceDirectory, includeAttributeData);
        }

        public XmlMapping AddMapping(DirectoryInfo sourceDir, bool includeAttributeData = false)
        {
            StringBuilder sb = new StringBuilder();
            if (sourceDir == null || !sourceDir.Exists)
            {
                throw new ArgumentException("Source Directory must exist.", nameof(sourceDir));
            }

            XmlMapping existingMapping = this.ImportMappings.FirstOrDefault(m => m.Source.Name == sourceDir.FullName);
            if (existingMapping != null)
            {
                throw new ArgumentException($"Can not add duplicate mapping with name \"{sourceDir.FullName}\". XML Mappings must have unique paths.", nameof(sourceDir));
            }

            XmlMapping mapping = new XmlMapping(sourceDir, includeAttributeData);
            this.addMapping(mapping);
            return mapping;
        }

        /// <summary>
        /// Add a new mapping to the ImportMappings
        /// </summary>
        /// <param name="mapping">Mapping to add</param>
        /// <param name="skipInvalid">Skip over mappings that don't relate to valid sources. If not skipped, invalid mappings will throw error.</param>
        public void AddMapping(XmlMapping mapping, bool skipInvalid = false)
        {
            if (mapping != null && mapping.Source != null)
            {
                this.addMapping(mapping);
            }
            else if (!skipInvalid)
            {
                throw new ArgumentException("A valid mapping relating to existing directory must be provided.", "mapping.Source.Name: " + mapping.Source.Name);
            }
        }

        public void RemoveMapping(string sourcePath)
        {
            XmlMapping mapping = this.ImportMappings.FirstOrDefault(m => m.Source.Name == sourcePath);
            if (mapping != null)
            {
                this.removeMapping(mapping);
            }
        }

        public void RemoveMapping(DirectoryInfo sourceDir)
        {
            XmlMapping mapping = this.ImportMappings.FirstOrDefault(m => m.Source.Name == sourceDir.FullName);
            if (mapping != null)
            {
                this.removeMapping(mapping);
            }
        }

        public void ClearMappings()
        {
            this.ImportMappings.Clear();
        }

        public void SetMappings(List<XmlMapping> mappings)
        {
            this.ClearMappings();
            foreach (var mapping in mappings)
            {
                this.AddMapping(mapping, true);
            }
        }

        /// <summary>
        /// Retrieves all XML files in a given directory and stores them to an instance variable
        /// </summary>
        /// <param name="path">The path to retrieve the files from</param>
        public FileInfo[] GetAllXMLFiles(string path)
        {
            if (!Directory.Exists(path)) { return null; }

            var dir = new DirectoryInfo(path);
            return dir.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
        }
        #endregion

        #region Constructor
        public XmlImportJob()
        {
            this.ImportMappings = new List<XmlMapping>();
            this.MappingSources = new XmlMappingSourceList();
            this.ImportType = Enumerations.ImportJobType.Xml;
            this.DoWorkEvent += this.initializeImport;
        }

        public XmlImportJob(XmlMappingSourceList sources) : this()
        {
            if (!sources.Any())
            {
                throw new ArgumentException("ImportSources cannot be empty.", nameof(sources));
            }

            this.MappingSources = sources;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Method to simplify addMapping
        /// </summary>
        /// <param name="mapping">Mapping to add</param>
        private void addMapping(XmlMapping mapping)
        {
            this.ImportMappings.Add(mapping);
        }

        /// <summary>
        /// Method to simplify removeMapping
        /// </summary>
        /// <param name="mapping">Mapping to remove</param>
        private void removeMapping(XmlMapping mapping)
        {
            this.ImportMappings.Remove(mapping);
        }

        private void importData(XmlMapping currentMapping)
        {
            SetParent(currentMapping);

            var template = (SCItems.TemplateItem)this.GetItem(currentMapping.TemplateID);
            var ds = currentMapping.Source.Source;
            this._currentItemIndex = 0;
            for (; _currentItemIndex < this.currentFileList.Length; this._currentItemIndex++)
            {
                if (checkContinue())
                {
                    // run process to import the actual data
                    try
                    {
                        // load the xml document from file into an xdocument
                        var currentFileItem = this.currentFileList[this._currentItemIndex];
                        var currentXdoc = new XmlDocument();
                        currentXdoc.Load(currentFileItem.FullName);
                        // select name element by xpath
                        var nameElement = currentXdoc.SelectSingleNode(currentMapping.NameNodePath);
                        var name = nameElement == null ? "" : nameElement.Value ?? nameElement.InnerText;
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            LogStatus("Name and Parent cannot be blank.", currentFileItem.Name);
                            continue;
                        }
                        if (currentMapping.CleanNames)
                        {
                            name = SCItems.ItemUtil.ProposeValidItemName(name);
                        }
                        // get parent according to import settings
                        var parent = this.getParent(currentMapping, currentXdoc);
                        // only proceed if a parent and name are present since they are required by Sitecore
                        if (string.IsNullOrWhiteSpace(name) || parent == null)
                        {
                            LogStatus("Name and Parent cannot be blank.", currentFileItem.Name);
                            continue;
                        }
                        var newItem = template.CreateItemFrom(name, parent);
                        fileToItemList.Add(currentFileItem.FullName, newItem.ID);   // this allows quick reference to all processed files related to a Sitecore item
                        var fields = newItem.GetEditableFields();

                        using (new SC.SecurityModel.SecurityDisabler())
                        {
                            using (new SCItems.EditContext(newItem))
                            {
                                foreach (var mapping in currentMapping.FieldMappings)
                                {
                                    try
                                    {
                                        var field = fields.FirstOrDefault(f => f.Name.ToLower() == mapping.FieldName.ToLower());
                                        if (field != null)
                                        {
                                            var fieldElement = currentXdoc.SelectSingleNode(mapping.XPath);
                                            var fieldValue = fieldElement == null ? "" : fieldElement.Value ?? fieldElement.InnerText;
                                            if (!string.IsNullOrWhiteSpace(fieldValue))
                                            {
                                                field.SetValueByType(fieldValue);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogStatus($"[Field: {mapping.FieldName}] {ex.Message}", currentFileItem.Name);
                                    }
                                }
                            }
                        }

                        this.CurrentPercent = ((this._currentItemIndex + 1) * 100) / this.currentFileList.Length;
                        if (this.IsAsync)
                        {
                            this.BgWorker.ReportProgress(this.CurrentPercent, currentMapping.Source.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogStatus(ex.Message);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private SCItems.Item getParent(XmlMapping mappingData, XmlDocument currentXDoc)
        {
            if (mappingData.ParentIDSource == Enumerations.ParentSource.Global)
            {
                return this._parent;
            }
            else
            {
                var parentElement = currentXDoc.SelectSingleNode(mappingData.ParentNodePath);

                var parentValue = parentElement == null ? "" : parentElement.Value ?? parentElement.InnerText;

                Guid parentGuid = Guid.Empty;

                // test for GUID ID as Parent value and return matching item if parse successful
                if (Guid.TryParse(parentValue, out parentGuid))
                {
                    return this.GetItem(parentGuid);
                }

                // Here, Parent value is not a GUID so it could be a path. Use it to check for matching path in SuccessList and return matching ID.
                if (!string.IsNullOrWhiteSpace(parentValue))
                {
                    var fullParentPath = this.getFullPath(mappingData.Source.Source.FullName, parentValue);

                    SCData.ID parentId;
                    if (fileToItemList.TryGetValue(fullParentPath, out parentId) && !SCData.ID.IsNullOrEmpty(parentId))
                    {
                        return this.GetItem(parentId);
                    }
                }
            }
            return null;
        }

        private void initializeImport(object sender, DoWorkEventArgs e)
        {
            try
            {
                loadFileCounts();   // load counts before base initialize to ensure progress percentages are accurate
                InitializeImportBase();
                fileToItemList = new Dictionary<string, SCData.ID>(StringComparer.OrdinalIgnoreCase);   // start with a fresh list each time a new job starts

                foreach (var mapping in this.ImportMappings)
                {
                    this.CurrentPercent = 0;
                    this._currentItemIndex = 0;
                    this.CurrentMappingName = mapping.Source.Name;
                    if (checkContinue())
                    {
                        if (validateData(mapping))
                        {
                            this.importData(mapping);
                            this.LogStatus("Completed import of Mapping: " + this.CurrentMappingName);
                        }
                        else
                        {
                            this.ImportStatus = Enumerations.JobStatus.Error;
                        }
                    }
                    else
                    {
                        if (this.IsAsync)
                        {
                            e.Cancel = true;
                        }
                        this.ImportStatus = Enumerations.JobStatus.Canceled;
                        break;
                    }
                }
                if (this.IsJobRunning)
                {
                    this.ImportStatus = Enumerations.JobStatus.Completed;
                }
            }
            catch(Exception ex)
            {
                this.LogStatus(ex.Message);
                this.ImportStatus = Enumerations.JobStatus.Error;
            }
        }

        /// <summary>
        /// Load total count of XML files for each mapping that exists
        /// </summary>
        private void loadFileCounts()
        {
            foreach(var mapping in this.ImportMappings)
            {
                var dir = mapping.Source.Source;
                if (dir.Exists)
                {
                    mapping.Source.FileCount = dir.GetFiles("*.xml", SearchOption.TopDirectoryOnly).Length;
                }
            }
        }

        /// <summary>
        /// Validate required values for Import Job
        /// </summary>
        /// <returns>True if validation succeeds</returns>
        private bool validateData(XmlMapping mappingData)
        {
            var errorList = new List<string>();
            bool pathExists = true;
            if (mappingData.Source?.Source == null || !mappingData.Source.Source.Exists)
            {
                pathExists = false;
                string mapPath = mappingData.Source?.Source == null ? "Path is empty" : mappingData.Source.Source.FullName;
                errorList.Add($"Data source must be a valid directory. Verify the source path exists. Path: {mapPath}");
            }

            if (pathExists)
            {
                this.currentFileList = mappingData.Source.Source.GetFiles("*.*", SearchOption.AllDirectories);
                if (this.currentFileList == null || this.currentFileList.Length <= 0)
                {
                    errorList.Add("Source directory must contain valid XML files. No files were found.");
                }
            }

            if (mappingData.ParentIDSource == Enumerations.ParentSource.Global && mappingData.GlobalParentID == Guid.Empty)
            {
                errorList.Add("A valid GlobalParentID must be specified when TemplateMapSource is specified as Global");
            }

            if (errorList.Any())
            {
                foreach (var error in errorList)
                {
                    LogStatus(error);
                }
                return false;
            }

            return true;
        }

        private string getFullPath(string basePath, string filePath)
        {
            var fullPath = filePath;

            if (!Path.IsPathRooted(filePath))
            {
                var pathParts = Path.Combine(basePath, filePath);
                fullPath = Path.GetFullPath(pathParts);
            }
            return fullPath;
        }

        #endregion
    }
}
