using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics.ReportBuild
{
    public class CaCeReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public CaCeReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }
        public Dictionary<string, int> Ca { get; set; } = new();
        public Dictionary<string, int> Ce { get; set; } = new();
        public string FileTag { get; set; } = "CaCe";

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<h3 class=\"my-4\">CBO</h3>");

            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<thead>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th style = \"width:80%\" scope = \"col\" > Категория </th>");
            stringBuilder.Append("<th style = \"width:20%\" scope = \"col\" > Ca </th>");
            stringBuilder.Append("<th style = \"width:20%\" scope = \"col\" > Ce </th>");


            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");

            foreach (var vertex in Ca)
            {
                //Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{vertex.Key}</th>");
                //var colomnClass = vertex.b < GlobalBoundaryValues.BoundaryValues.CBO ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.Append($"<td >{vertex.Value}</th>");
                stringBuilder.Append($"<td >{Ce[vertex.Key]}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }
    }
}
