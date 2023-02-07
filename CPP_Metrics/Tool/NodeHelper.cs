using Antlr4.Runtime.Tree;
using System.Diagnostics.CodeAnalysis;

namespace CPP_Metrics.Tool
{
    public static class NodeHelper
    {
        public static Identifier GetTheTypeName([NotNull] this CPP14Parser.TheTypeNameContext context)
        {
            Identifier identifier = new Identifier();
            var contextChild = context.children.First();
            if (contextChild is CPP14Parser.ClassNameContext className)
            {
                if (className.children.First() is CPP14Parser.SimpleTemplateIdContext simpleTemplate)
                {
                    identifier = new TemplateIdentifier();
                    identifier.Name = simpleTemplate.children.First().GetText();
                    var templateArgumentList = simpleTemplate.children
                                                .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
                }
                else
                {
                    identifier.Name = className.children.First().GetText();
                }
            }
            else if (contextChild is CPP14Parser.EnumNameContext enumName)
            {
                identifier.Name = enumName.children.First().GetText();
            }
            else if (contextChild is CPP14Parser.TypedefNameContext typedefName)
            {
                identifier.Name = typedefName.children.First().GetText();

            }
            else if (contextChild is CPP14Parser.SimpleTemplateIdContext simpleTemplate)
            {
                identifier = new TemplateIdentifier();
                identifier.Name = simpleTemplate.children.First().GetText();
                var templateArgumentList = simpleTemplate.children
                                                .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
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
