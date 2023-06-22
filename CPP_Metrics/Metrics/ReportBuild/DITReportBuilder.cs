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

            stringBuilder.AppendLine("""
                <h3 class="my-4">Высота дерева наследования </h3>
                  <p>< strong > DIT(Depth of Inheritance tree) </strong> — глубина дерева наследования(наибольший путь по иерархии классов к данному классу от
                    класса - предка), чем больше, тем лучше, так как при большей глубине увеличивается абстракция данных, уменьшается насыщенность класса методами, однако при достаточно
                    большой глубине сильно возрастает сложность понимания и написания программы.</p>
                    <p>
                    <strong> NOC(Number of children) </strong> — количество потомков(непосредственных), чем больше, тем выше абстракция данных.
                    </p>
                  <table class="table mt-4">
                """);

           
            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<th style = \"width:80%\" scope = \"col\" > Класс </th>");
            stringBuilder.AppendLine("<th style = \"width:20%\" scope = \"col\" > DIT </th>");
            stringBuilder.AppendLine("<th style = \"width:20%\" scope = \"col\" > NOC </th>");

            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</thead>");

            stringBuilder.AppendLine("<tbody>");

            foreach (var vertex in DITGraph.Verticies.OrderByDescending(x => x.ParenCount))
            {
                //Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine($"<td>{vertex.Name}</th>");
                var colomnClass = vertex.ParenCount < GlobalBoundaryValues.BoundaryValues.DIT ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.AppendLine($"<td {colomnClass}>{vertex.ParenCount}</th>");
                
                colomnClass = DITGraph[vertex].Count < GlobalBoundaryValues.BoundaryValues.NOC ? "class=\"table-success\"" : "class=\"table-danger\"";
                stringBuilder.AppendLine($"<td {colomnClass}>{DITGraph[vertex].Count}</th>");

                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");

            return stringBuilder.ToString();
        }

    }
}
