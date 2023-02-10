using Antlr4.Runtime.Tree;
using CPP_Metrics.Types.Context;
using System.Diagnostics.CodeAnalysis;

namespace CPP_Metrics.Tool
{
    public static class NodeHelper
    {
        public static CPPType GetTheTypeName([NotNull] this CPP14Parser.TheTypeNameContext context)
        {
            CPPType identifier = new CPPType();
            var contextChild = context.children.First();
            if (contextChild is CPP14Parser.ClassNameContext className)
            {
                if (className.children.First() is CPP14Parser.SimpleTemplateIdContext simpleTemplate)
                {
                    identifier.TypeName = simpleTemplate.children.First().GetText();
                    var templateArgumentList = simpleTemplate.children
                                                .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
                    identifier.TemplateNames = new List<CPPType>();
                }
                else
                {
                    identifier.TypeName = className.children.First().GetText();
                }
            }
            else if (contextChild is CPP14Parser.EnumNameContext enumName)
            {
                identifier.TypeName = enumName.children.First().GetText();
            }
            else if (contextChild is CPP14Parser.TypedefNameContext typedefName)
            {
                identifier.TypeName = typedefName.children.First().GetText();
            }
            else if (contextChild is CPP14Parser.SimpleTemplateIdContext simpleTemplate)
            {
                identifier.TypeName = simpleTemplate.children.First().GetText();
                var templateArgumentList = simpleTemplate.children
                                                .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
                identifier.TemplateNames = new List<CPPType>();
            }
            return identifier;
        }
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
