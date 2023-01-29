using Antlr4.Runtime.Tree;

namespace CPP_Metrics
{
    public static class IParseTreeHelper
    {
        public static IList<IParseTree> GetChildren(this IParseTree parseTree)
        {
            IList<IParseTree> children = new List<IParseTree>();
            for (int i = 0; i < parseTree.ChildCount; i++)
            {
                children.Add(parseTree.GetChild(i));
            }
            return children;
        }
    }
}
