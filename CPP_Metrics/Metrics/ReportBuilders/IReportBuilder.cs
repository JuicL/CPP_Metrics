using System.Text;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    public interface IReportBuilder
    {
        public ReportInfo ReportInfo { get; }
        public string FileTag { get; }
        public string GenerateBody();
        public string ReportBuild()
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
