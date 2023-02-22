

using System.Text;

namespace CPP_Metrics.Metrics.ReportBuild
{
    internal class CyclomaticReportBuilder : IReportBuilder
    {
        public string Header { get ; set ; }
        public string Footer { get; set; }
        public string OutPath { get; set; }
        public List<CyclomaticComplexityInfo> CyclomaticComplexityInfos { get; }

        public CyclomaticReportBuilder(List<CyclomaticComplexityInfo> cyclomaticComplexityInfos)
        {
            CyclomaticComplexityInfos = cyclomaticComplexityInfos.OrderByDescending(x => x.CyclomaticComplexityValue).ToList();
        }
        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<table>");
            stringBuilder.Append("<thead>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append($"<th>{ "Функция" }</th>");
            stringBuilder.Append($"<th>{"Значение"}</th>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");
            // GroupBy className
            var Classes = CyclomaticComplexityInfos.GroupBy(x=> x.FunctionInfo.NestedNames.Last().TypeName).ToList();
            
            foreach (var item in CyclomaticComplexityInfos)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{item.FunctionInfo.Text}</th>");
                stringBuilder.Append($"<td>{item.CyclomaticComplexityValue}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        public string ReportBuild()
        {
            throw new NotImplementedException();
        }
    }
}
