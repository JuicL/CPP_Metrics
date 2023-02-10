using Antlr4.Runtime.Tree;

namespace CPP_Metrics.Tool
{
    // TODO : Аналайз через поиск в глубину (обратный)
    public static class Analyzer
    {
        public static IParseTree? FindUp(IParseTree startNode, IParseTree element)
        {
            IParseTree? result = startNode;
            while (result != null)
            {
                if (result.GetType().Equals(element))
                {
                    return result;
                }
                result = result.Parent;
            }
            return result;
        }

        public static IList<IParseTree> FindDown(IParseTree startNode, Func<IParseTree, bool> func)
        {
            IList<IParseTree> result = new List<IParseTree>();
            Stack<IParseTree> bag = new Stack<IParseTree>();
            bag.Push(startNode);
            while (bag.Any())
            {
                var vertex = bag.Pop();
                if (func(vertex))
                //if (vertex.GetType().Equals(element))
                {
                    result.Add(vertex);
                }
                for (int i = vertex.ChildCount - 1; i >= 0; --i)
                {
                    bag.Push(vertex.GetChild(i));
                }
            }
            return result;
        }

        public static void Analyze<T>(IParseTree tree, IParseTreeVisitor<T> visitor)
        {
            Stack<IParseTree> bag = new Stack<IParseTree>();
            bag.Push(tree);
            while (bag.Any())
            {
                var vertex = bag.Pop();
                Console.WriteLine($"{vertex.GetType().Name}:{visitor.GetType().Name}");
                bool result = vertex.Accept((IParseTreeVisitor<bool>)visitor);

                if (result == true)
                {
                    for (int i = vertex.ChildCount - 1; i >= 0; --i)
                    {
                        bag.Push(vertex.GetChild(i));
                    }
                }
            }
        }

        public static void AnalyzeR<T>(IParseTree tree, IParseTreeVisitor<T> visitor)
        {
            Queue<IParseTree> bag = new Queue<IParseTree>();
            bag.Enqueue(tree);
            while (bag.Any())
            {
                var vertex = bag.Dequeue();

                bool result = vertex.Accept((IParseTreeVisitor<bool>)visitor);

                if (result == true)
                {
                    for (int i = vertex.ChildCount - 1; i >= 0; --i)
                    {
                        bag.Enqueue(vertex.GetChild(i));
                    }
                }
            }
        }
        public static void AnalyzeR<T>(IList<IParseTree> tree, IParseTreeVisitor<T> visitor)
        {
            foreach (var vertex in tree.Reverse())
            {
                Analyze(vertex, visitor);
            }
        }
        public static void Analyze<T>(IList<IParseTree> tree, IParseTreeVisitor<T> visitor)
        {
            foreach (var vertex in tree)
            {
                Analyze(vertex, visitor);
            }
        }
    }
}
