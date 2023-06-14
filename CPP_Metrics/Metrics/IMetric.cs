using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;

namespace CPP_Metrics.Metrics
{
    public enum MessageType
    {
        Error,Warning, 
    }
   

    public class MetricMessage
    { 
        public MessageType MessageType { get; set; }
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
