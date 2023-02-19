
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.OOP
{
    //for classSpecifierContext
    
    public class ClassStructVisitor : CPP14ParserBaseVisitor<bool>
    {
        public ClassStructInfo ClassStructInfo { get; set; } = new ClassStructInfo();
       
        public override bool VisitClassKey([NotNull] CPP14Parser.ClassKeyContext context)
        {
            var classKey = context.children.First();
            ClassStructInfo.ClassKey = classKey.GetText();
            return false;
        }

        public override bool VisitMemberSpecification([NotNull] CPP14Parser.MemberSpecificationContext context)
        {
            // Body class
            ClassStructInfo.Body = context;
            return false;
        }

        public override bool VisitClassHeadName([NotNull] CPP14Parser.ClassHeadNameContext context)
        {
            var className = context.className();

            var identifier = className.Identifier();
            if (identifier is null) // Not simple Identifier, continue visits
            {
                return true;
            }
            ClassStructInfo.Name = identifier.GetText();
            Console.WriteLine($"Имя класса {ClassStructInfo.Name}");
            return false;
        }

        // Шаблонный класс
        public override bool VisitSimpleTemplateId([NotNull] CPP14Parser.SimpleTemplateIdContext context)
        {
            var templateName = context.children.First(); // templateName
            ClassStructInfo.Name = templateName.GetText();
            Console.WriteLine($"Имя класса {ClassStructInfo.Name}");
            //Если понадобиться парсить имена шаблонов
            var templateArgumentList = context.children.FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);

            return false;
        }
       
        // Наследование
        public override bool VisitBaseSpecifier([NotNull] CPP14Parser.BaseSpecifierContext context)
        {
            var baseClass = new BasedClassInfo();
            var virtualMarker = context.Virtual();
            if (virtualMarker is not null)
                baseClass.VirtualMarker = true;
            var accesSpecifier = context.accessSpecifier();
            if(accesSpecifier is not null)
            {
                baseClass.AccesSpecifier = AccesSpecifierHelper.GetAccesSpecifier(accesSpecifier.children.First().GetText());
            }

            var classOrDeclTypeContext = context.baseTypeSpecifier().classOrDeclType();
            var className = classOrDeclTypeContext.className();
            var nestedNameSpecifier = classOrDeclTypeContext.nestedNameSpecifier();
            if(nestedNameSpecifier is not null)
            {
                var nestedVisitor = new NestedNameSpecifierVisitor();
                Analyzer.Analyze(nestedNameSpecifier, nestedVisitor);
                baseClass.NestedNames.AddRange(nestedVisitor.NestedNames);
            }

            if (className is not null)
            {
                var classNameChild = className.GetChildren().First();
                if(classNameChild is CPP14Parser.SimpleTemplateIdContext simpleTemplate)// SimpleTemplate
                {
                    var templateName = simpleTemplate.children.First();
                    baseClass.TypeName = templateName.GetText();
                    baseClass.TemplateNames = new List<CPPType>();
                    var templateArgumentList = simpleTemplate.children
                                                    .FirstOrDefault(x => x is CPP14Parser.TemplateArgumentListContext);
                    //Console.WriteLine($"--Base Class {ClassStructInfo.BaseClasses.Last().TypeName}");

                }
                else// Identifire
                {
                    baseClass.TypeName = classNameChild.GetText();

                }
                ClassStructInfo.BaseClasses.Add(baseClass);
                Console.WriteLine($"--Base Class {ClassStructInfo.BaseClasses.Last().TypeName}");
            }
            else
            {
                //TODO: decltypeSpecifier
            }

            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
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