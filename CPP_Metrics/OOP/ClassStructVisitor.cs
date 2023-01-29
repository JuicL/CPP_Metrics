
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics.OOP
{
    //for classSpecifierContext
    public class ClassStructVisitor : CPP14ParserBaseVisitor<bool>
    {
        /// <summary>
        /// Class or Struct
        /// </summary>
        public string ClassKey { get; private set; }

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
        public override bool VisitClassKey([NotNull] CPP14Parser.ClassKeyContext context)
        {
            var classKey = context.children.First();
            ClassKey = classKey.GetText();
            return false;
        }

        public override bool VisitMemberSpecification([NotNull] CPP14Parser.MemberSpecificationContext context)
        {
            //
            return false;
        }
        public override bool VisitClassHeadName([NotNull] CPP14Parser.ClassHeadNameContext context)
        {
            Console.Write("Имя ");
            var classNameContex = Analyzer.FindDown(context, x => x is CPP14Parser.ClassNameContext).FirstOrDefault();
            Console.WriteLine(classNameContex.GetText());

            return false;
        }
        // Наследование от шаблонного класса
        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            var inheritances = context.children.First(); // templateName
            Console.Write("Наследование ");
            Console.WriteLine(inheritances.GetText());
            //Если понадобиться парсить имена шаблонов
            var templateArgumentList = context.children.FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);

            return false;
        }
        // Наследование от простого класса
        public override bool VisitBaseSpecifier([NotNull] CPP14Parser.BaseSpecifierContext context)
        {
            var inheritances = Analyzer.FindDown(context, x => x is CPP14Parser.ClassNameContext);

            foreach (var item in inheritances)
            {
                if (item.GetChild(0) is CPP14Parser.SimpleTemplateIdContext template)
                {
                    VisitSimpleTemplateId(template);
                    return false;
                }
                else
                {
                    Console.Write("Наследование ");
                    Console.WriteLine(item.GetText());
                }

            }
            return false;
        }
    }
}