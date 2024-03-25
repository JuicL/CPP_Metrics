using System.Globalization;
using System.Text;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    public class InstabilityReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public InstabilityReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }

        public string FileTag { get; set; } = "Instability";

        public Dictionary<string, decimal> Instability { get; set; }
        public CaCeMetric CaCeMetric;
        public ClassAbstractionMetric ClassAbstraction;
        public Dictionary<string, decimal> D { get; set; } = new();

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<script>");
            stringBuilder.Append("x=[");
            foreach (var item in Instability)
                stringBuilder.Append($"{item.Value.ToString(CultureInfo.InvariantCulture)}, ");
            stringBuilder.Append("];");

            stringBuilder.Append("y=[");
            foreach (var item in Instability)
                stringBuilder.Append($"{ClassAbstraction.Result[item.Key].ToString(CultureInfo.InvariantCulture)}, ");
            stringBuilder.Append("];");

            stringBuilder.Append("text=[");
            foreach (var item in Instability)
                stringBuilder.Append($" \'{item.Key}\', ");
            stringBuilder.Append("];");


            stringBuilder.AppendLine("</script>");
            stringBuilder.AppendLine($"<h3 class=\"my-4\">Расстояние от главной последовательности</h3>");
            stringBuilder.AppendLine("<div id ='myDiv' style=\"outline: 1px solid #000;\"></div>");
            stringBuilder.AppendLine("""
                <script src='https://cdn.plot.ly/plotly-2.24.1.min.js'></script>
                <script>
                    var data = [{
                              x: x,
                              y: y,
                              text: text,
                              type: 'scatter',
                              mode: 'markers',
                              marker: {
                                color: 'rgb(17, 157, 255)',
                                size: 15,
                              },
                              showlegend: false
                            }]
                        var layout = {
                          xaxis: {range:[0,1],
                            title: {
                            text: 'I (Нестабильность)',
                            font: {
                              family: 'Arial, monospace',
                              size: 22,
                              color: '#000000'
                              }
                            }
                          },
                          yaxis: {
                            range:[0,1],
                            title: {
                            text: 'A (Абстрактность)',
                            font: {
                              family: 'Arial, monospace',
                              size: 22,
                              color: '#000000'
                              }
                            }}
                        };
                
                        Plotly.newPlot('myDiv', data,layout)
                </script>
                """);

            stringBuilder.AppendLine($"""
                <div class="container pt-4"><h3 class="my-4">Центростремительное и центробежное сцепление</h3>
                  <p>
                    <strong>Ca:</strong> Центростремительное сцепление. Количество классов вне этой категории, которые зависят от классов внутри этой категории.
                  </p>
                  <p><strong>Ce:</strong>Центробежное сцепление. Количество классов внутри этой категории, которые зависят от классов вне этой категории.
                  </p>
                 </p>
                  <p><strong>I:</strong> Нестабильность категории. Вычисляется как:  Ce / (Ca+Ce)
                  </p>
                 </p>
                  <p><strong>A:</strong> Абстрактность категории. Равняется частному: общее количество классов на количество абстрактных классов в категории.
                  </p>
                 </p>
                  <p><strong>D:</strong> Нормализованное расстояние от главной последовательности. Вычисляется как D = |A + I - 1|.
                  </p>
                <h3 class="my-4">Нестабильность категории</h3>

                  <table class="table">
                   <thead>
                    <tr>
                   <th style = "width:80%" scope = "col" > Категория </th>
                   <th style = "width:80%" scope = "col" > Ca </th>
                   <th style = "width:80%" scope = "col" > Ce </th>
                   <th style = "width:80%" scope = "col" > I </th>
                   <th style = "width:80%" scope = "col" > A </th>
                   <th style = "width:80%" scope = "col" > D </th>


                   </tr>
                   </thead>

                   <tbody>
                """);


            foreach (var item in Instability)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{item.Key}</th>");
                stringBuilder.Append($"<td>{CaCeMetric.Ca[item.Key]}</th>");
                stringBuilder.Append($"<td>{CaCeMetric.Ce[item.Key]}</th>");
                stringBuilder.Append($"<td >{item.Value}</th>");
                stringBuilder.Append($"<td >{ClassAbstraction.Result[item.Key]}</th>");
                stringBuilder.Append($"<td >{D[item.Key]}</th>");


                stringBuilder.Append("</tr>");
            }

            stringBuilder.AppendLine($"""     
                   </tbody>
                   </table>
                """);

            return stringBuilder.ToString();
        }
    }

}
