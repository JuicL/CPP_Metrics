

using System.Text;

namespace CPP_Metrics.Metrics.ReportBuild
{
    public class GeneralPageReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; }

        public List<FileInfo> ProjectFiles = new();
        public List<MetricMessage> MetricMessages { get; set; } = new();
        public GeneralPageReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }

        public string FileTag { get; } = "index";

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<h4> Количество ошибок в проекте {MetricMessages.Count}</h4>");
            foreach (var item in MetricMessages)
            {
                stringBuilder.Append("<div class=\"alert alert-danger\">");
                stringBuilder.Append($"<strong>Error!</strong> {item.Message}</div>");
            }

            stringBuilder.Append("<h3 class=\"my-4\">Файлы проекта</h3>");
            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<tbody>");
            foreach (var file in ProjectFiles)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{file.FullName}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            
            return stringBuilder.ToString();
        }
    }
}
