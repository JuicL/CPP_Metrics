

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.CyclomaticComplexity
{
    public class CyclomaticComplexityMetric
    {
        public List<Pair<FunctionInfo, CyclomaticGraph>> Cyclomatic = new List<Pair<FunctionInfo, CyclomaticGraph>>();

        public CyclomaticComplexityMetric()
        {
        }
        public int GetCyclomaticComplexity(CyclomaticGraph graph)
        {
            var P = 1; // Todo: Компонет связности
            return graph.Edges.Count - graph.Verticies.Count + 2*P;
        }
        public void Analyze(IParseTree three)
        {
            CyclomaticComplexityFunctionVisitor visitor = new CyclomaticComplexityFunctionVisitor();
            Analyzer.Analyze(three, visitor);
            Cyclomatic.AddRange(visitor.Cyclomatic);
        }
    }
}
