using Antlr4.Runtime.Tree;
using CPP_Metrics.Visitors.OOP;

namespace CPP_Metrics.Types.Context
{
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
        public int Line { get; set; }
        public IParseTree FunctionBody { get; set; }
        public List<BaseContextElement> References { get; set; } = new List<BaseContextElement>();
    }

}
