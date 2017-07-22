using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Specialized.Content.Import.SupportClasses;

namespace Specialized.Content.Import.Interfaces
{
    public interface IImportJob
    {
        Enumerations.ImportJobType ImportType { get; set; }
        List<StatusMessage> ImportMessages { get; set; }
        Enumerations.JobStatus ImportStatus { get; set; }

        bool IsJobRunning { get; }
        bool IsJobCanceled { get; }

        void ImportData();
        void CancelImport();
    }
    public interface IStatusMessage
    {
        Enumerations.ImportJobType JobType { get; set; }
        string MappingName { get; set; }
        string Message { get; set; }
        string CurrentItem { get; set; }
    }

    public interface IHasImportStatus<T> where T: IStatusMessage
    {
        List<T> ImportMessages { get; set; }
        Enumerations.JobStatus ImportStatus { get; set; }
    }

    public interface IMapping { }
}
