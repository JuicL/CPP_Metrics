

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics
{
    /// <summary>
    /// Это больше про парсинг SimpleDeclaration...
    /// </summary>
    /*
     Рассмотрены случа объявления переменной и функции(с указанием namespace). Переменная типа класса. Присваивание переменной(классу) функции/переменной
     */
    public class SimpleDeclarationContextVisitor : CPP14ParserBaseVisitor<bool>
    {
        public List<string> VariableNames = new();
        public List<string> CallFuncNames = new();
        public List<string> DeclFuncNames = new();

        private IList<Parameter>? Parameters = null;

        public string? DeclSpecifierSeqType = null;
        private bool isParameters = false;
        private bool isDeclSpec;
        private bool isParametersBraces = false;
        private bool isNameSpaced = false;
        private IList<string> OutVariable = new List<string>();
        public SimpleDeclarationContextVisitor(List<string> outVariables)
        {
            OutVariable = outVariables;
        }
        public SimpleDeclarationContextVisitor()
        {
            this.isDeclSpec = false;
        }
        public SimpleDeclarationContextVisitor(bool isDeclSpec)
        {
            this.isDeclSpec = isDeclSpec;
        }
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            return false;  // Декларация класса находится в ветке simpleDeclaration
        }
        public new bool VisitParametersAndQualifiers([NotNull] CPP14Parser.ParametersAndQualifiersContext context)
        {
            isParameters = true;
            var parameterDeclaration = context.children.FirstOrDefault(x => x is CPP14Parser.ParameterDeclarationClauseContext)
                ?.GetChildren()
                .FirstOrDefault(x => x is CPP14Parser.ParameterDeclarationListContext)
                ?.GetChildren()
                .Where(x => x is CPP14Parser.ParameterDeclarationContext).ToList();
            if (parameterDeclaration is null) return false;
            var parameters = new List<Parameter>();
            foreach (var item in parameterDeclaration)
            {
                var parameterVisitor = new ParameterVisitor();
                Analyzer.Analyze(item, parameterVisitor);
                parameters.Add(parameterVisitor.Parameter);
            }
            Parameters = parameters;
            return false;
        }
        public override bool VisitDeclSpecifierSeq([NotNull] CPP14Parser.DeclSpecifierSeqContext context)
        {
            this.isDeclSpec = true;
            var visitor = new TypeVisitor();
            Analyzer.Analyze(context, visitor);
            if(visitor.Type.Length > 0)
            {
                DeclSpecifierSeqType = visitor.Type;
            }
            return false;
        }
        public override bool VisitPostfixExpression([NotNull] CPP14Parser.PostfixExpressionContext context)
        {
            //TODO: это не только вызов функции
            
            return true;
        }

        public override bool VisitClassName([NotNull] CPP14Parser.ClassNameContext context)
        {
            var name = context.children.First();
            //VariableNames.Add(name.GetText());

            return true;
        }
        
        public override bool VisitNoPointerDeclarator([NotNull] CPP14Parser.NoPointerDeclaratorContext context)
        {
            if (context.Parent is CPP14Parser.NoPointerDeclaratorContext)
                return true;
            // Случай, когда после имени стоят <(> or <)>
            var parameters = context.children.FirstOrDefault(x => x is CPP14Parser.ParametersAndQualifiersContext);
            if (parameters is not null)
                VisitParametersAndQualifiers((CPP14Parser.ParametersAndQualifiersContext)parameters);

            isParametersBraces = context.children.Count > 1 ? true : false;
            
            return true;
        }
        public override bool VisitIdExpression([NotNull] CPP14Parser.IdExpressionContext context)
        {
            // Случай,когда перед именем стоит <::>
            isNameSpaced = context.children.First() is CPP14Parser.QualifiedIdContext;
            

            return true;
        }
        /// <summary>
        /// Проверка что идет после оператора доступа <.> метод или поле класса
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsFucntionPostfix(IParseTree context)// Доходим до первого Postfix смотрим у него родителя postfix(если он таковым является и есть ли у него дочерние узлы с "()"
        {
            var parent = context.Parent;
            while(parent != null)
            {
                if(parent is CPP14Parser.PostfixExpressionContext)
                {
                    if (parent?.Parent is CPP14Parser.PostfixExpressionContext parentParentContext)
                    {
                        var braces = parentParentContext.children.Where(x => x.GetText() == "(" || x.GetText() == ")");
                        if(braces.Count() == 2)
                        {
                            return true;
                        }
                        break;
                    }
                }
                parent = parent.Parent;
            }
            
            return false;
        }
        private List<string> SelectParameters(IList<Parameter> parameters, IList<string> variables)
        {
            return parameters is null ? new List<string>() : parameters?.Where(p => p.Name is null && variables.Contains(p.Type)).Select(x => x.Type).ToList();
        }
        private bool IsDeclarationFunction(IList<Parameter> parameters,IList<string> variables)
        {
            //пустые скобки
            if (isParameters && parameters is null) 
                return true;
            foreach(var parameter in parameters)
            {
                // если имя и тип не нулевые значит точно декларация
                if(parameter.Name is not null && parameter.Type is not null)
                {
                    return true;
                }
                // имя null проверяем type на равенство с именем переменной
                if (parameter.Name is null && variables.Contains(parameter.Type))
                {
                    return false;
                }
            }
            return false;
        }
        
        /* 
         * TODO : Точнее разделить случаи
         *        Шаблоны!
         *        define!
        Identifier
        | operatorFunctionId
        | conversionFunctionId
        | literalOperatorId
        | Tilde (className | decltypeSpecifier)
        | templateId;
        */
        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            if (context.children.Count != 1) // если индификатор
            {
                return false;
            }
            var name = context.children.First().GetText();

            // проверка декларация функции по параметрам внутри parametrsBraced
            if(isDeclSpec && isParameters && IsDeclarationFunction(Parameters, OutVariable))
            {
                DeclFuncNames.Add(name);
                return false;
            }

            if (isNameSpaced && isParametersBraces || !isDeclSpec && isParametersBraces)// Если :: и () //() без declSpec -- функция
            {
                CallFuncNames.Add(name);
                return false;
            }


            if (IsFucntionPostfix(context))// для postfix
            {
                CallFuncNames.Add(name);
                return true;
            }
            VariableNames.Add(DeclSpecifierSeqType + " " + name);
            VariableNames.AddRange(SelectParameters(Parameters, OutVariable));

            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
