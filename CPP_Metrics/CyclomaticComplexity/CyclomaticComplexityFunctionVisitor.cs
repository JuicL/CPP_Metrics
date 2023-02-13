

using Antlr4.Runtime.Misc;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.CyclomaticComplexity
{
    public class CyclomaticComplexityFunctionVisitor : CPP14ParserBaseVisitor<bool>
    {
        // Function inner classes
        public List<Pair<FunctionInfo, CyclomaticGraph>> Cyclomatic = new List<Pair<FunctionInfo, CyclomaticGraph>>();

        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            var functionInfoVisitor = new FunctionDefinitionVisitor();
            Analyzer.Analyze(context, functionInfoVisitor);
            var functionInfo = functionInfoVisitor.FunctionInfo;

            CyclomaticGraph graph = new CyclomaticGraph();

            CyclomaticComplexityVisitor visitor = new CyclomaticComplexityVisitor(graph, null, null);
            Analyzer.AnalyzeR(context, visitor);
            graph.CreateEdge(graph.Head, visitor.Last is null ? graph.Tail : visitor.Last);
            
            Cyclomatic.Add(new Pair<FunctionInfo, CyclomaticGraph>(functionInfo, graph));

            return false;
        }

    }
}
