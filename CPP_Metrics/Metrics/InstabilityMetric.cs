using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.Contracts;
using CPP_Metrics.Metrics.ReportBuilders;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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

        public ClassAbstractionMetric? ClassAbstraction;
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
            ((InstabilityReportBuilder)ReportBuilder).Instability = Instability;
            ((InstabilityReportBuilder)ReportBuilder).CaCeMetric = CaCeMetric;
            ((InstabilityReportBuilder)ReportBuilder).ClassAbstraction = ClassAbstraction;
            ((InstabilityReportBuilder)ReportBuilder).D = D;
            ReportBuilder.ReportBuild();
            return "";
        }

        public bool Handle(List<IMetric> metrics)
        {
            CaCeMetric = (CaCeMetric?)metrics.SingleOrDefault(x => x is CaCeMetric);
            ClassAbstraction = (ClassAbstractionMetric?)metrics.SingleOrDefault(x => x is ClassAbstractionMetric);
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

}
