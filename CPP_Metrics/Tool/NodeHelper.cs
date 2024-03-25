using Antlr4.Runtime.Tree;
using CPP_Metrics.Types.Reflection;
using CPP_Metrics.Visitors;
using System.Diagnostics.CodeAnalysis;

namespace CPP_Metrics.Tool
{
    public static class NodeHelper
    {
        public static NameSpaceInfo GetNameSpaceInfo([NotNull] this CPP14Parser.NamespaceDefinitionContext context)
        {
            NameSpaceInfo namespaceInfo = new NameSpaceInfo();
            var inline = context.Inline();
            if (inline != null)
                namespaceInfo.IsInline = true;
            var indetifer = context.Identifier();
            if (indetifer == null)
                namespaceInfo.Name = "";
            else
                namespaceInfo.Name = indetifer.GetText();

            return namespaceInfo;

        }
        public static CPPType GetTheTypeName([NotNull] this CPP14Parser.TheTypeNameContext context)
        {
            CPPType identifier = new CPPType();
            var contextChild = context.children.First();
            if (contextChild is CPP14Parser.ClassNameContext className)
            {
                if (className.children.First() is CPP14Parser.SimpleTemplateIdContext simpleTemplate)
                {
                    var templateName = simpleTemplate.templateName(); // typeName actually
                    identifier.TypeName = templateName.GetText();
                    
                    var templateVisitor = new TemplateArgumentVisitor();
                    var templateArgumentList = simpleTemplate.templateArgumentList();
                    if(templateArgumentList is not null)
                        Analyzer.Analyze(templateArgumentList, templateVisitor);

                    identifier.TemplateNames = templateVisitor.Types;

                    //identifier.TypeName = simpleTemplate.children.First().GetText();
                    //var templateArgumentList = simpleTemplate.children
                    //                            .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
                    //identifier.TemplateNames = new List<CPPType>();
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
