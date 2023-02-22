
namespace CPP_Metrics.Metrics.ReportBuild
{
    public interface IReportBuilder
    {
        public static string Header { get; set; }
        public static string Footer { get; set; }
        public string OutPath { get; set; }
        public string GenerateBody();
        public string ReportBuild();
    }


}
