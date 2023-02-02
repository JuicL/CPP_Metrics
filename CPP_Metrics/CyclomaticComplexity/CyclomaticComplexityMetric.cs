

using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;

namespace CPP_Metrics.CyclomaticComplexity
{
    public class CyclomaticComplexityMetric
    {
        public readonly CyclomaticGraph graph;
        public CyclomaticComplexityMetric()
        {
            graph = new CyclomaticGraph();
        }

        public void Analyze(IParseTree three)
        {
            CyclomaticComplexityVisitor visitor = new CyclomaticComplexityVisitor(graph,null,null);
            Analyzer.Analyze(three, visitor);
            
            graph.CreateEdge(graph.Head, visitor.Last is null? graph.Tail: visitor.Last);
        }
    }
}
