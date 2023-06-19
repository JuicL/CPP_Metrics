using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics.ReportBuild
{
    public class CBOReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public CBOReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }

        public string FileTag { get; set; } = "CBO";
        public List<CBOResult> Result { get; set; } = new();

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<h3 class=\"my-4\">CBO</h3>");

            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<thead>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th style = \"width:80%\" scope = \"col\" > Класс </th>");
            stringBuilder.Append("<th style = \"width:20%\" scope = \"col\" > Значение </th>");

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");

            foreach (var vertex in Result.OrderByDescending(x => x.Value))
            {
                //Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{vertex.ClassName}</th>");
                var colomnClass = vertex.Value < GlobalBoundaryValues.BoundaryValues.CBO ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.Append($"<td {colomnClass}>{vertex.Value}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }
    }
}
