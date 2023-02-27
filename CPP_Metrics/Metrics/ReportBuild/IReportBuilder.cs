
using System.Globalization;
using System.Text;

namespace CPP_Metrics.Metrics.ReportBuild
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

            // Создать файл с названием
            var fileIncludesPath = Path.Combine(ReportInfo.OutPath, FileTag + ".html");

            FileStream fileStream = new FileStream(fileIncludesPath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            // Записать туда head + body() + footer
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Close();
            fileStream.Close();

            // Вернуть путь до файла
            return fileIncludesPath;
        }
    }


}
