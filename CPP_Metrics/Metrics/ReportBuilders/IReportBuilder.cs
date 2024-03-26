using CPP_Metrics.Types;
using System.Text;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    public abstract class IReportBuilder
    {
        public ReportInfo ReportInfo { get; }
        public Config Config { get; }
        public List<MetricMessage> MetricMessages { get; }
        public string FileTag { get; }
        public abstract string GenerateBody();
        public virtual string ReportBuild()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ReportInfo.Header);
            stringBuilder.Append(GenerateBody());
            stringBuilder.Append(ReportInfo.Footer);

            var fileIncludesPath = Path.Combine(ReportInfo.OutPath, FileTag + ".html");

            FileStream fileStream = new FileStream(fileIncludesPath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Close();
            fileStream.Close();

            return fileIncludesPath;
        }
    }


}
