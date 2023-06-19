using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;

namespace CPP_Metrics.Metrics
{
    public enum MessageType
    {
        Error,Warning, 
    }

    /*
    Rules id
    + CyclomaticComplexityId
    + DitId
    + CBOId
    - CA
    - CE
    + NocId
    +SLOCCommentedPercendId
    +SLOCEmptyPercendId
        

     */



    public class MetricMessage
    { 
        public MessageType MessageType { get; set; }
        public string Id { get; set; }
        public string Severity { get; set; } = "warning";
        // Подробнее
        public string Verbose { get; set; } = "";
        
        public string Message { get; set; }
    }

    public interface IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public string GenerateReport();
        public void Finalizer();
        public bool Handle(ProcessingFileInfo processingFileInfo);
        public void Save(DbContextMetrics dbContext,Solution solution);
        public List<MetricMessage> Messages { get; set; }
    }

    public interface ICombineMetric: IMetric
    {
        public bool Handle(List<IMetric> metrics);

    }

}
