using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{
        //I = Ce / (Ce + Ca)

        // A
        //
        //      I(Instability)

        //D = A + I - 1 
    public class InstabilityMetric : ICombineMetric
    {
        public CaCeMetric? CaCeMetric;

        public ClassAbstraction? ClassAbstraction;
        public Dictionary<string, decimal> Instability { get; set; } = new();
        public Dictionary<string, decimal> D { get; set; } = new();


        public IReportBuilder ReportBuilder { get; set; }
        public List<MetricMessage> Messages { get; set; } = new();
        public InstabilityMetric(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        public void Finalizer()
        {
        }

        public string GenerateReport()
        {
            if (CaCeMetric == null || ClassAbstraction == null) return "";
            ((InstabilityReport)ReportBuilder).Instability = Instability;
            ((InstabilityReport)ReportBuilder).CaCeMetric = CaCeMetric;
            ((InstabilityReport)ReportBuilder).ClassAbstraction = ClassAbstraction;


            ReportBuilder.ReportBuild();
            return "";
        }

        public bool Handle(List<IMetric> metrics)
        {
            CaCeMetric = (CaCeMetric?)metrics.SingleOrDefault(x => x is CaCeMetric);
            ClassAbstraction = (ClassAbstraction?)metrics.SingleOrDefault(x => x is ClassAbstraction);
            if (CaCeMetric == null || ClassAbstraction == null) return false;

            foreach (var Ce in CaCeMetric.Ce)
            {
                var caTake = CaCeMetric.Ca.TryGetValue(Ce.Key,out int ca);
                if (caTake == false) continue;
                decimal instability;
                decimal denominator = (Ce.Value + ca);
                if (denominator == 0)
                {
                    instability = 0;
                }
                else
                {
                     instability = Ce.Value / denominator;
                }
                Instability.Add(Ce.Key, instability);
                var abstractTake = ClassAbstraction.Result.TryGetValue(Ce.Key, out decimal abstraction);

                if(abstractTake == true)
                {
                    var d = instability + abstraction - 1;
                    D.Add(Ce.Key, d);
                }
            }

            return true;
        }

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            return false;
        }

        public void Save(DbContextMetrics dbContext, Solution solution)
        {
            
        }
    }


    public class InstabilityReport : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public InstabilityReport(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }

        public string FileTag { get; set; } = "Instability";

        public Dictionary<string, decimal> Instability { get; set; }
        public CaCeMetric CaCeMetric;
        public ClassAbstraction ClassAbstraction;

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
            stringBuilder.AppendLine("<div id ='myDiv'> </div>");
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
                          xaxis: {range:[0,1]},
                          yaxis: {range:[0,1]}
                        };
                
                        Plotly.newPlot('myDiv', data,layout)
                </script>
                """);

         stringBuilder.AppendLine($"""
                <h3 class="my-4">Нестабильность категории</h3>

                  <table class="table">
                   <thead>
                    <tr>
                   <th style = "width:80%" scope = "col" > Категория </th>
                   <th style = "width:80%" scope = "col" > Ca </th>
                   <th style = "width:80%" scope = "col" > Ce </th>
                   <th style = "width:80%" scope = "col" > I </th>
                   <th style = "width:80%" scope = "col" > A </th>

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
