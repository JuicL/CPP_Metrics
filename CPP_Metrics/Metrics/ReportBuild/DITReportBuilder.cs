using CPP_Metrics.Types.DIT;
using System.Text;


namespace CPP_Metrics.Metrics.ReportBuild
{
    public class DITReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; }
        public DITGraph DITGraph { get; set; }
        public string FileTag { get; } = "DIT";

        public DITReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }
        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<h3 class=\"my-4\">Высота наследования</h3>");

            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<thead>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th style = \"width:80%\" scope = \"col\" > Класс </th>");
            stringBuilder.Append("<th style = \"width:20%\" scope = \"col\" > DIT </th>");
            stringBuilder.Append("<th style = \"width:20%\" scope = \"col\" > NOC </th>");

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");

            foreach (var vertex in DITGraph.Verticies.OrderByDescending(x => x.ParenCount))
            {
                //Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{vertex.Name}</th>");
                var colomnClass = vertex.ParenCount < GlobalBoundaryValues.BoundaryValues.DIT ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.Append($"<td {colomnClass}>{vertex.ParenCount}</th>");
                
                colomnClass = DITGraph[vertex].Count < GlobalBoundaryValues.BoundaryValues.NOC ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.Append($"<td {colomnClass}>{DITGraph[vertex].Count}</th>");

                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

    }
}
