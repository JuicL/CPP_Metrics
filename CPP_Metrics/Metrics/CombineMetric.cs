using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{
    public class InstabilityMetric : ICombineMetric
    {
        //I = Ce / (Ce + Ca)

        // A
        //
        //      I(Instability)

        //D = A + I - 1 
        private CaCeMetric? CaCeMetric;

        private ClassAbstraction? ClassAbstraction;
        public Dictionary<string, decimal> Instability { get; set; } = new();
        public Dictionary<string, decimal> D { get; set; } = new();


        public IReportBuilder ReportBuilder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<MetricMessage> Messages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public InstabilityMetric(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        public void Finalizer()
        {
        }

        public string GenerateReport()
        {
            ((InstabilityReport)ReportBuilder).Instability = Instability;
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
                if (ca == 0)
                {
                    instability = 0;
                }
                else
                {
                     instability = Ce.Value / (Ce.Value + ca);
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

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"""
                <h3 class=\"my-4\">Нестабильность категории</h3>

                  <table class=\"table\">
                   <thead>
                    <tr>
                   <th style = \"width:80%\" scope = \"col\" > Класс </th>
                   <th style = \"width:20%\" scope = \"col\" > Значение </th>

                   </tr>
                   </thead>

                   <tbody>
                """);


            foreach (var item in Instability)
            {
                stringBuilder.Append("<tr>");

                stringBuilder.Append($"<td>{item.Key}</th>");
                stringBuilder.Append($"<td >{item.Value}</th>");
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
