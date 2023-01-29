
using Antlr4.Runtime.Misc;

namespace CPP_Metrics
{
    public class ParameterVisitor : CPP14ParserBaseVisitor<bool>
    {
        public Parameter Parameter { get; private set; } = new Parameter();

        public override bool VisitDeclSpecifierSeq([NotNull] CPP14Parser.DeclSpecifierSeqContext context)
        {
            var typeVisitor = new TypeVisitor();
            Analyzer.Analyze(context, typeVisitor);
            if(typeVisitor.Type.Length >0)
            {
                Parameter.Type = typeVisitor.Type;
            }
            else { Parameter.Type = null; }

            return false;
        }

        public override bool VisitDeclarator([NotNull] CPP14Parser.DeclaratorContext context)
        {
            var name = context.GetTerminalNodes();
            if(name.Count == 0)
                return false;
            Parameter.Name = name.First().GetText();
            
            return false;

            //var visitor = new NameVisitor();
            //Analyzer.Analyze(context, visitor);
            //Parameter.Name = visitor.Name;
            //return false;
        }

    }
}
