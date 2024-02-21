namespace CPP_Metrics.Types.Context
{
    public class BaseContextElement
    {
        public BaseContextElement()
        {
            Source = CurrentSource;
        }

        public IDictionary<string,List<FunctionInfo>> FunctionDeclaration = new Dictionary<string,List<FunctionInfo>>();

        public IDictionary<string, Variable> VariableDeclaration = new Dictionary<string, Variable>();

        public IDictionary<string, ClassStructInfo> TypeDeclaration = new Dictionary<string, ClassStructInfo>();

        public List<CPPType> UsedClasses = new List<CPPType>();

        public Guid Guid { get; set; }
        // Доступные Using's
        public List<UsingNamespace> UsingNamespaces { get; set; } = new List<UsingNamespace>();
        public List<SimpleUsing> SimpleUsing { get; set; } = new List<SimpleUsing>();
        public Dictionary<string,CPPType> AliasDeclaration { get; set; } = new Dictionary<string,CPPType>();

        public BaseContextElement? Paren { get; set; } = null;
        public List<BaseContextElement> Children { get; set; } = new List<BaseContextElement>();
        public IEnumerable<BaseContextElement> Filter(Func<BaseContextElement, bool> func)
        {
            IList<BaseContextElement> filteredСollection = new List<BaseContextElement>();

            Stack<BaseContextElement> bag = new Stack<BaseContextElement>();
            bag.Push(this);
            while (bag.Any())
            {
                var vertex = bag.Pop();
                var result = func.Invoke(vertex);
                
                if (result == true)
                {
                    filteredСollection.Add(vertex);
                }

                for (int i = vertex.Children.Count - 1; i >= 0; --i)
                {
                    bag.Push(vertex.Children[i]);
                }
            }

            return filteredСollection;
        }
        public static void Clear()
        {
            if (GeneralNameSpace is null) return;
            GeneralNameSpace = null;
        }

        
        public static BaseContextElement GetGeneralNameSpace()
        {
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + (GeneralNameSpace is null).ToString());
            if(GeneralNameSpace is null)
            {
                var generalNamespace = new NamespaceContext();
                NameSpaceInfo spaceInfo = new()
                {
                    Name = "::",
                    IsInline = false,
                };
                generalNamespace.NameSpaceInfo = spaceInfo;
                GeneralNameSpace = generalNamespace;
            }
            return GeneralNameSpace;
        }
        [ThreadStatic]
        private static BaseContextElement? GeneralNameSpace;
        public string Source { get; set; } = "";
        [ThreadStatic]
        public static string CurrentSource  = "";
    }

}
