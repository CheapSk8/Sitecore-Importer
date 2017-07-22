using System;
using System.Collections.Generic;
using System.ComponentModel;

using Specialized.Content.Import.Interfaces;
using System.Runtime.Serialization;

namespace Specialized.Content.Import.SupportClasses
{
    /// <summary>
    /// EventArgs to use for OnProgressChange
    /// </summary>
    public class ImportProgressChangedEventArgs : EventArgs
    {
        public string CurrentMapping { get; private set; }
        public bool IsCanceled { get; private set; }
        public int PercentCompleted { get; private set; }

        public ImportProgressChangedEventArgs(int percent, string currentMapping, bool canceled)
        {
            this.CurrentMapping = currentMapping;
            this.IsCanceled = canceled;
            this.PercentCompleted = percent;
        }
    }

    /// <summary>
    /// EventArgs to use for OnProgressEnd
    /// </summary>
    public class ImportProgressEndedEventArgs
    {
        public int PercentCompleted { get; internal set; }

        public bool IsCanceled { get; private set; }
        public Exception Error { get; private set; }
        public object Result { get; private set; }

        public ImportProgressEndedEventArgs(int percent, object result, Exception error = null, bool isCanceled = false)
        {
            this.PercentCompleted = percent;
            this.Result = result;
            this.Error = error;
            this.IsCanceled = isCanceled;
        }

        public ImportProgressEndedEventArgs(RunWorkerCompletedEventArgs args)
            : this(0, args.Result, args.Error, args.Cancelled)
        {
        }

        public static implicit operator ImportProgressEndedEventArgs(RunWorkerCompletedEventArgs args)
        {
            return new ImportProgressEndedEventArgs(args);
        }
    }

    /// <summary>
    /// Used for logging errors
    /// </summary>
    public class StatusMessage : IStatusMessage
    {
        public Enumerations.ImportJobType JobType { get; set; }
        public string MappingName { get; set; }
        public string Message { get; set; }
        public string CurrentItem { get; set; }
    }

    #region Enumerations Class
    public class Enumerations
    {
        /// <summary>
        /// enum to define source of Parent ID mapping
        /// </summary>
        [DataContract(Name = "ParentSource")]
        public enum ParentSource
        {
            [EnumMember]
            Global,
            [EnumMember]
            Item
        }
        [DataContract(Name = "JobStatus")]
        public enum JobStatus
        {
            [EnumMember]
            Running,
            [EnumMember]
            Idle,
            [EnumMember]
            Canceled,
            [EnumMember]
            Completed,
            [EnumMember]
            Error
        }

        [DataContract(Name = "ImportJobType")]
        public enum ImportJobType
        {
            [EnumMember]
            Xls,
            [EnumMember]
            Xml
        }
    }
    #endregion
}
