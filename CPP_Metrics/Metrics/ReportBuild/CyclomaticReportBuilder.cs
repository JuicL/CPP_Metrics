﻿

using System.Text;

namespace CPP_Metrics.Metrics.ReportBuild
{
    internal class CyclomaticReportBuilder : IReportBuilder
    {

        public List<CyclomaticComplexityInfo> CyclomaticComplexityInfos { get; set; }
        public ReportInfo ReportInfo { get; }
        public string FileTag { get; } = "Cyclomatic";
        public CyclomaticReportBuilder(ReportInfo reportInfo)
        {
            //CyclomaticComplexityInfos = cyclomaticComplexityInfos.OrderByDescending(x => x.CyclomaticComplexityValue).ToList();
            ReportInfo = reportInfo;
        }
        public string GenerateBody()
        {
            
            CyclomaticComplexityInfos = CyclomaticComplexityInfos.OrderByDescending(x => x.CyclomaticComplexityValue).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append("<h3 class=\"my-4\">Цикломатическая сложность</h3>");

            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<thead>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th style = \"width:60%\" scope = \"col\" > Функция </th>");
            stringBuilder.Append("<th style = \"width:30%\" scope = \"col\" > Функция </th>");
            stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Значение </th>");

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");
            // GroupBy className
            //var Classes = CyclomaticComplexityInfos.GroupBy(x=> x.FunctionInfo.NestedNames.Last().TypeName).ToList();
            
            foreach (var item in CyclomaticComplexityInfos)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{item.FunctionInfo.Text}</th>");
                stringBuilder.Append($"<td>{item.FileName}</th>");
                var colomnClass = item.CyclomaticComplexityValue < 12 ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.Append($"<td {colomnClass}>{item.CyclomaticComplexityValue}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }
        
    }
}
