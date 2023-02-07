
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;


namespace CPP_Metrics.OOP
{
    //for classSpecifierContext
    public class ClassStructVisitor : CPP14ParserBaseVisitor<bool>
    {
        /// <summary>
        /// Class or Struct
        /// </summary>
        public string ClassKey { get; private set; }
        public string ClassName { get; private set; }
        public IList<string> BaseClasses { get; private set; } = new List<string>();
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
            // Body class
            return false;
        }
        public override bool VisitClassHeadName([NotNull] CPP14Parser.ClassHeadNameContext context)
        {
            var className = (CPP14Parser.ClassNameContext)context.children.FirstOrDefault(x => x is CPP14Parser.ClassNameContext);
            var test = className.Identifier();
            var test2 = className.simpleTemplateId();
            var name = className.GetTerminalNodes();
            if (name.Count == 0) // Not simple Identifier, continue visits
            {
                return true;
            }
            ClassName = name.First().GetText();
            Console.WriteLine($"Имя класса {ClassName}");
            return false;
        }

        // Шаблонный класс
        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            var templateName = context.children.First(); // templateName
            ClassName = templateName.GetText();
            Console.WriteLine($"Имя класса {ClassName}");
            //Если понадобиться парсить имена шаблонов
            var templateArgumentList = context.children.FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);

            return false;
        }
       
        // Наследование
        public override bool VisitBaseSpecifier([NotNull] CPP14Parser.BaseSpecifierContext context)
        {

            var className = Analyzer.FindDown(context, x => x is CPP14Parser.ClassNameContext).FirstOrDefault();
            if(className is not null)
            {
                var classNameChild = className.GetChildren().First();
                if(classNameChild is CPP14Parser.SimpleTemplateIdContext simpleTemplate)// SimpleTemplate
                {
                    var templateName = simpleTemplate.children.First();
                    BaseClasses.Add(templateName.GetText());
                    var templateArgumentList = simpleTemplate.children
                                                    .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
                    Console.WriteLine($"--Base Class {BaseClasses.Last()}");

                }
                else// Identifire
                {
                    BaseClasses.Add(classNameChild.GetText());
                    Console.WriteLine($"--Base Class {BaseClasses.Last()}");
                }


            }
            else
            {
                //decltypeSpecifier
            }

            return false;
        }
    }
}
/*
 * class Test1{};class Test11{};class Test12{};

class Test13 :public Test11, private Test12
{

};
template<typename T1>
class Test2
{
};

template<typename T1>
class Test3 : Test2<int>
{

};

template<typename T3>
class Test4 :public Test2<T3>
{

};
 */