using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;

namespace CPP_Metrics.Metrics
{
    public interface IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public string GenerateReport();
        public void Finalizer();
        public bool Handle(ProcessingFileInfo processingFileInfo);
    }
}
