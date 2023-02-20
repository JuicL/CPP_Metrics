using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using Facads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{
    public class CylomaticComplexity : IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public CylomaticComplexity(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        public CylomaticComplexity()
        {
        }

        public List<CyclomaticComplexityInfo> FunctionCyclomatic = new();

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            CyclomaticComplexityFunctionVisitor visitor = new();
            Analyzer.Analyze(processingFileInfo.ProcessingFileTree, visitor);
            foreach (var cyclomatic in visitor.Cyclomatic)
            {
                var cyclomaticComplexityInfo = new CyclomaticComplexityInfo();
                
            }
            FunctionCyclomatic.AddRange(visitor.Cyclomatic);

            return true;
        }

        // Not needed
        public void Finalizer()
        {
        }

        public string GenerateReport()
        {
            foreach (var item in FunctionCyclomatic)
            {
                Console.WriteLine($"{item.FunctionInfo.Name} {item.CyclomaticComplexityValue}");
            }
            return "";
        }

    }
}
