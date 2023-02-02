using Antlr4.Runtime.Tree;

namespace CPP_Metrics.Tool
{
    public static class NodeHelper
    {
        public static IList<IParseTree> GetTerminalNodes(this IParseTree node)
        {
            var result = new List<IParseTree>();
            for (int i = 0; i < node.ChildCount; i++)
            {
                if (node.GetChild(i) is TerminalNodeImpl)
                {
                    result.Add(node.GetChild(i));
                }
            }
            return result;
        }
    }
}
