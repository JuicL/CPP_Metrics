using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;

namespace CPP_Metrics.Metrics
{
    public interface IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public bool Handle(ProcessingFileInfo processingFileInfo);
        public void Finalizer();
        public string GenerateReport();
    }
}
