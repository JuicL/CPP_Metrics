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

            stringBuilder.AppendLine("""
                <div class="container pt-4"><h3 class="my-4">Cцепление между классами</h3>
                  <p>
                    <strong>CBO (Coupling between object classes)</strong> — 
                    сцепление между классами, показывает количество классов, с которыми связан исходный класс
                  </p>
                  <table class="table mt-4">
                """);

            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<th style = \"width:80%\" scope = \"col\" > Класс </th>");
            stringBuilder.AppendLine("<th style = \"width:20%\" scope = \"col\" > Значение </th>");

            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</thead>");

            stringBuilder.Append("<tbody>");

            foreach (var vertex in Result.OrderByDescending(x => x.Value))
            {
                //Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine($"<td>{vertex.ClassName}</th>");
                var colomnClass = vertex.Value < GlobalBoundaryValues.BoundaryValues.CBO ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.AppendLine($"<td {colomnClass}>{vertex.Value}</th>");
                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");

            return stringBuilder.ToString();
        }
    }
}
