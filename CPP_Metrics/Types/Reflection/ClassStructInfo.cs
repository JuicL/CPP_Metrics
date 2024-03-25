using Antlr4.Runtime.Tree;

namespace CPP_Metrics.Types.Reflection
{
    public class ClassStructInfo
    {
        public string ClassKey { get; set; }
        public string Name { get; set; }
        public IList<string> TemplateNames { get; set; }
        public List<FunctionInfo> Methods { get; set; } = new();
        public List<FieldsInfo> Fields { get; set; } = new();
        public List<CPPType> Nested { get; set; } = new();
        public List<BasedClassInfo> BaseClasses { get; private set; } = new();
        public bool IsAbstract 
        { get
            {
                foreach (var method in Methods)
                {
                    if(method.IsPure)
                        return true;
                }
                return false;
            } 
        }
        public bool IsTemplate { get; set; }
        public bool IsDeclaration { get; set; } // class <className> 
        public string GetFullName()
        {
            string res = "";
            res += GetNamespace();
            if(!res.EndsWith("::"))
            {
                res += "::";
            }
            res += Name;
            return res;
        }

        public string GetNamespace()
        {
            
            string res = "";
            foreach (var item in Nested)
            {
                if (item.TypeName.Equals("::"))
                    continue;
                res += "::";
                res += item.TypeName;
            }

            if (res.Length == 0)
                res += "::";
            return res;
        }
        public string FileName { get; set; }
        public int Line { get; set; }
        public IParseTree Body { get; set; }
        public List<CPPType> UsedTypes { get; set; } = new List<CPPType>();
    }

}
