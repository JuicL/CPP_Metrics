

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Metrics;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.CyclomaticComplexity
{
    public static class CyclomaticComplexityMetricHelper
    {
        public static int GetCyclomaticComplexity(this CyclomaticGraph graph)
        {
            var condition = 0;
            foreach (var item in graph.Verticies)
            {
                condition += item.Value;
            }
            var P = 1; // Todo: Компонет связности
            return (graph.Edges.Count - graph.Verticies.Count + 2 * P) + condition;
        }
    }

    public class CyclomaticComplexityMetric
    {
        public List<CyclomaticComplexityInfo> Cyclomatic = new List<CyclomaticComplexityInfo>();

        public CyclomaticComplexityMetric()
        {
        }
        
        public void Analyze(IParseTree three)
        {
            CyclomaticComplexityFunctionVisitor visitor = new CyclomaticComplexityFunctionVisitor();
            Analyzer.Analyze(three, visitor);
            Cyclomatic.AddRange(visitor.Cyclomatic);
        }
    }
}
