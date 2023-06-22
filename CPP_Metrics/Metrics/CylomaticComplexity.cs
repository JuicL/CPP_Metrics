using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using Facads;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{
    public class CylomaticComplexity : IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public List<MetricMessage> Messages { get; set; } = new();

        public CylomaticComplexity(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        public ConcurrentBag<CyclomaticComplexityInfo> FunctionCyclomatic { get; set; } = new();

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            CyclomaticComplexityFunctionVisitor visitor = new();
            Analyzer.Analyze(processingFileInfo.ProcessingFileTree, visitor);
            foreach (var item in visitor.Cyclomatic)
            {
                item.FileName = processingFileInfo.FileInfo.Name;
                FunctionCyclomatic.Add(item);
            }

            return true;
        }

        public void Finalizer()
        {
            foreach (var item in FunctionCyclomatic)
            {
                if(item.CyclomaticComplexityValue > GlobalBoundaryValues.BoundaryValues.Complexity)
                {
                    Messages.Add(new MetricMessage() 
                    {  Id = "CyclomaticComplexityId", 
                        MessageType = MessageType.Error,
                        Message = $"Сyclomatic complexity value is too high {item.FunctionInfo.Text} allowed {GlobalBoundaryValues.BoundaryValues.Complexity}." +
                        $"\n File {item.FileName}"
                    });
                }
            }
        }

        public string GenerateReport()
        {
            ((CyclomaticReportBuilder)ReportBuilder).CyclomaticComplexityInfos = FunctionCyclomatic;
            ReportBuilder.ReportBuild();

           
            return "";
        }

        public void Save(DbContextMetrics dbContext, Solution solution)
        {
            foreach (var cyclomatic in FunctionCyclomatic)
            {
                var value = new MetricValue()
                {
                    SolutionID = solution.ID,
                    MetricDirectoryID = (int)dbContext.GetIdMetric("Cyclomatic"),
                    FileName = cyclomatic.FileName,
                    ObjectName = cyclomatic.FunctionInfo.Name,
                    Value = cyclomatic.CyclomaticComplexityValue
                };
                dbContext.MetricValues.Add(value);
            }
            dbContext.SaveChanges();
        }
    }
}
