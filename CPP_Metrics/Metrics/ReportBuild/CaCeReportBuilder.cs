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

            stringBuilder.AppendLine("""
                <div class="container pt-4"><h3 class="my-4">Центростремительное и центробежное сцепление</h3>
                  <p>
                    <strong>
                      Ca: 
                    </strong>
                    Центростремительное сцепление. Количество классов вне этой категории, которые зависят от классов внутри этой категории.
                  </p>
                  <p>
                    <strong>
                      Ce: 
                    </strong>
                    Центробежное сцепление. Количество классов внутри этой категории, которые зависят от классов вне этой категории.
                  </p>
                  <table class="table mt-4">
                """);

            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<th style = \"width:80%\" scope = \"col\" > Категория </th>");
            stringBuilder.AppendLine("<th style = \"width:20%\" scope = \"col\" > Ca </th>");
            stringBuilder.AppendLine("<th style = \"width:20%\" scope = \"col\" > Ce </th>");


            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</thead>");

            stringBuilder.AppendLine("<tbody>");

            foreach (var vertex in Ca)
            {
                //Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine($"<td>{vertex.Key}</th>");
                //var colomnClass = vertex.b < GlobalBoundaryValues.BoundaryValues.CBO ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.AppendLine($"<td >{vertex.Value}</th>");
                stringBuilder.AppendLine($"<td >{Ce[vertex.Key]}</th>");
                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");

            return stringBuilder.ToString();
        }
    }
}
