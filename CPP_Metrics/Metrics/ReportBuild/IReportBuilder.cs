
namespace CPP_Metrics.Metrics.ReportBuild
{
    public interface IReportBuilder
    {
        public string Header { get; set; }
        public string Footer { get; set; }
        public void GenerateBody();
        public string ReportBuild();
    }
}
