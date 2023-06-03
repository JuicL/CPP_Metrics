using Antlr4.Runtime.Tree;
using CPP_Metrics.OOP;
using System.Collections.Generic;

namespace CPP_Metrics.Types.Context
{
    public class NameSpaceInfo
    {
        public bool IsInline { get; set; } = false;
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new();
        public string FullName()
        {
            string res = "";
            foreach (var item in Nested)
            {
                res += item.TypeName;
                if (item.TypeName.Equals("::"))
                    continue;
                res += "::";
            }
            res += Name;
            return res;
        }
    }

    public class FieldsInfo : Variable
    {
        public AccesSpecifier AccesSpecifier { get; set; }
    }
    public class BasedClassInfo : CPPType
    {
        public bool VirtualMarker { get; set; } = false;
        public AccesSpecifier AccesSpecifier { get; set; }
    }
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
            res += Name;
            return res;
        }
        public string GetNamespace()
        {
            string res = "";
            foreach (var item in Nested)
            {
                res += item.TypeName;
                if (item.TypeName.Equals("::"))
                    continue;
                res += "::";
            }
           
            return res;
        }

        public IParseTree Body { get; set; }
        public List<CPPType> UsedTypes { get; set; } = new List<CPPType>();
    }

    public class FunctionInfo
    {
        public string Text { get; set; }
        public CPPType? ReturnType { get; set; }
        public string Name { get; set; }
        public IList<Parameter> Parameters { get; set; } = new List<Parameter>();
        /// <summary>
        /// Имена шаблона
        /// </summary>
        public IList<string>? Templates { get; set; }
        public List<CPPType> NestedNames { get; set; } = new List<CPPType>();
        public AccesSpecifier? AccesSpecifier { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsMethod 
        { 
            get { return NestedNames.Count > 0; } 
            set { } 
        }
        public string GetFullName()
        {
            return "";
        }
        public bool Override { get; set; } = false;

        public bool Final { get; set; } = false;

        public bool IsPure { get; set; } = false;
        public IParseTree FunctionBody { get; set; }
        public List<BaseContextElement> References { get; set; } = new List<BaseContextElement>();
    }

    public enum ContextType
    {
        Variable,
        ClassStruct,
        Function,

    }

    // Фильтр
    // Обнуление контекста 
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

        public ContextType ContextType { get; set; }
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
            //if (GeneralNameSpace is null) return;
            //var generalNamespace = new NamespaceContext();
            //NameSpaceInfo spaceInfo = new()
            //{
            //    Name = "::",
            //    IsInline = false,
            //};
            //generalNamespace.NameSpaceInfo = spaceInfo;
            //GeneralNameSpace = generalNamespace;
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
    
    
    //temporarily
    public class SimpleUsing
    {
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new List<CPPType>();
        public ClassStructDeclaration? BaseContextElement { get; set; }
    }

    public class UsingNamespace
    {
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new List<CPPType>();
    }

    public class NamespaceContext : BaseContextElement
    {
        public NamespaceContext ParenNameSpace { get; set; }
        public NameSpaceInfo NameSpaceInfo { get; set; }
    }

    public class SimpleContext : BaseContextElement
    { }
    public class FunctionDeclaration : BaseContextElement 
    { 
        public FunctionInfo FunctionInfo { get; set; }
        
    }

    public class UsesVariable : BaseContextElement
    { }
    public class CallFunction : BaseContextElement 
    { }

    public class ClassStructDeclaration : BaseContextElement
    {
        public ClassStructInfo ClassStructInfo;
        
    }

    public class UsingDeclaration : BaseContextElement { }

}
