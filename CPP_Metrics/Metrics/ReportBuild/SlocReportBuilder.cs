using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics.ReportBuild
{
    internal class SlocReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public string FileTag { get; set; } = "Sloc";
        public Dictionary<FileInfo, SLocInfo> SlocMetrics { get; set; } = new();

        public SlocReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }
        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<h3 class=\"my-4\">Sloc</h3>");
            stringBuilder.Append("<h3 class=\"my-4\">Общее</h3>");

                stringBuilder.Append("<table class=\"table\">");
                stringBuilder.Append("<thead>");
                stringBuilder.Append("<tr>");

                    stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Lines </th>");
                    stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Comments </th>");
                    stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Empty </th>");
                    stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > %Comment </th>");
                    stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > %Empty </th>");

                stringBuilder.Append("</tr>");
                stringBuilder.Append("</thead>");
                stringBuilder.Append("<tbody>");
                    SLocInfo sLocInfo = SlocMetrics.Single(x => x.Key.Name == "|global|").Value;
            
                    stringBuilder.Append("<tr>");
                        stringBuilder.Append($"<td>{sLocInfo.Lines}</th>");
                        stringBuilder.Append($"<td>{sLocInfo.Commented}</th>");
                        stringBuilder.Append($"<td>{sLocInfo.EmptyLines}</th>");
                        stringBuilder.Append($"<td>{sLocInfo.PercentСomment}</th>");
                        stringBuilder.Append($"<td>{sLocInfo.PercentСomment}</th>");
                    stringBuilder.Append("</tr>");
            

                stringBuilder.Append("</tbody>");
                stringBuilder.Append("</table>");

            stringBuilder.Append("<h3 class=\"my-4\">Файлы проекта</h3>");

                stringBuilder.Append("<table class=\"table\">");
                stringBuilder.Append("<thead>");
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<th style = \"width:50%\" scope = \"col\" > Файл </th>");

                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Lines </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Comments </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Empty </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > %Comment </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > %Empty </th>");


                stringBuilder.Append("</tr>");
                stringBuilder.Append("</thead>");

                stringBuilder.Append("<tbody>");
                foreach (var item in SlocMetrics.Where(x => x.Key.Name != "|global|"))
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append($"<td>{item.Key.Name}</th>");
                    stringBuilder.Append($"<td>{item.Value.Lines}</th>");
                    stringBuilder.Append($"<td>{item.Value.Commented}</th>");
                    stringBuilder.Append($"<td>{item.Value.EmptyLines}</th>");
                    stringBuilder.Append($"<td>{item.Value.PercentСomment}</th>");
                    stringBuilder.Append($"<td>{item.Value.PercentСomment}</th>");
                    stringBuilder.Append("</tr>");
                }

                stringBuilder.Append("</tbody>");
                stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        
    }
}
