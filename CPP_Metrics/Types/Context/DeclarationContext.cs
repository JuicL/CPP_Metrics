using Antlr4.Runtime.Tree;
using CPP_Metrics.OOP;

namespace CPP_Metrics.Types.Context
{
    public class NameSpaceInfo
    {
        public bool IsInline { get; set; } = false;
        public string Name { get; set; }

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

        public bool IsTemplate { get; set; }
        public bool IsDeclaration { get; set; } // class <className> 

        public IParseTree Body { get; set; }
    }

    public class FunctionInfo
    {
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
        public bool IsMethod { get; set; }

        public bool Override { get; set; } = false;

        public bool Final { get; set; } = false;

        public bool IsPure { get; set; } = false;
        public IParseTree FunctionBody { get; set; }
    }

    public enum ContextType
    {
        Variable,
        ClassStruct,
        Function,

    }

    public class BaseContextElement
    {
        public BaseContextElement()
        {
            Source = CurrentSource;
        }

        public IDictionary<string,List<FunctionInfo>> FunctionDeclaration = new Dictionary<string,List<FunctionInfo>>();

        public IDictionary<string, Variable> VariableDeclaration = new Dictionary<string, Variable>();

        public IDictionary<string, ClassStructInfo> TypeDeclaration = new Dictionary<string, ClassStructInfo>();

        public Guid Guid { get; set; }
        // Доступные Using's
        public List<UsingNamespace> UsingNamespaces { get; set; } = new List<UsingNamespace>();
        public List<SimpleUsing> SimpleUsing { get; set; } = new List<SimpleUsing>();
        public Dictionary<string,CPPType> AliasDeclaration { get; set; } = new Dictionary<string,CPPType>();


        public ContextType ContextType { get; set; }
        public BaseContextElement? Paren { get; set; } = null;
        public List<BaseContextElement> Children { get; set; } = new List<BaseContextElement>();

        public static BaseContextElement GetGeneralNameSpace()
        {
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
        private static BaseContextElement? GeneralNameSpace;
        public  string Source { get; private set; } = "";
        public static string CurrentSource { get; set; } = "";
    }
    
    
    //temporarily
    public class SimpleUsing
    {
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new List<CPPType>();
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
