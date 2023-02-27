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


        public List<CyclomaticComplexityInfo> FunctionCyclomatic = new();

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            CyclomaticComplexityFunctionVisitor visitor = new();
            Analyzer.Analyze(processingFileInfo.ProcessingFileTree, visitor);
            foreach (var item in visitor.Cyclomatic)
            {
                item.FileName = processingFileInfo.FileInfo.Name;
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
            ((CyclomaticReportBuilder)ReportBuilder).CyclomaticComplexityInfos = FunctionCyclomatic;
            ReportBuilder.ReportBuild();

            //foreach (var item in
            //tionCyclomatic)
            //{
            //    Console.WriteLine($"{item.FunctionInfo.Text} {item.CyclomaticComplexityValue}");
            //}
            return "";
        }

    }
}
