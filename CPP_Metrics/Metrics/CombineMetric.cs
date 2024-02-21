using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.Contracts;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        private bool InCircle(Point point, decimal radios,decimal px, decimal py)
        {
            var res = ((px - point.X) * (px - point.X) + (py - point.Y) * (py - point.Y)) < radios * radios;
            return res;
        }

        public void Finalizer()
        {
            if (ClassAbstraction is null) return;

            var pointPain = new Point(0,0);
            var pointFutility = new Point(1, 1);

            foreach (var item in Instability)
            {
                var abstractTake = ClassAbstraction.Result.TryGetValue(item.Key, out decimal abstraction);
                if (abstractTake == false) continue;
                var pain = InCircle(pointPain,GlobalBoundaryValues.BoundaryValues.RadiusPain, item.Value, abstraction);
                if (pain == true)
                {
                    Messages.Add(new()
                    {
                        Id = "PainSectionId",
                        Message = $"Категория: {item.Key} находится в области \"боли\"",
                        MessageType = MessageType.Error
                    });
                }

                var futility = InCircle(pointFutility, GlobalBoundaryValues.BoundaryValues.RadiusFutility, item.Value, abstraction);
                if (futility == true)
                {
                    Messages.Add(new()
                    {
                        Id = "FutilitySectionId",
                        Message = $"Категория {item.Key} находится в области бесполезности",
                        MessageType = MessageType.Error
                    });
                }
            }

        }

        public string GenerateReport()
        {
            if (CaCeMetric == null || ClassAbstraction == null) return "";
            ((InstabilityReport)ReportBuilder).Instability = Instability;
            ((InstabilityReport)ReportBuilder).CaCeMetric = CaCeMetric;
            ((InstabilityReport)ReportBuilder).ClassAbstraction = ClassAbstraction;
            ((InstabilityReport)ReportBuilder).D = D;



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
                    var d = Math.Abs(instability + abstraction - 1);
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
            foreach (var item in Instability)
            {
                var value = new MetricValue()
                {
                    SolutionID = solution.ID,
                    MetricDirectoryID = (int)dbContext.GetIdMetric("I"),
                    FileName = "",
                    ObjectName = item.Key,
                    Value = item.Value
                };
                dbContext.MetricValues.Add(value);
            }
            dbContext.SaveChanges();
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
