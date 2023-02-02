

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

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
        public List<Variable> VariableNames = new();

        public List<string> CallFuncNames = new();

        public List<string> DeclFuncNames = new();

        private IList<Parameter>? Parameters = null;

        public string? DeclSpecifierSeqType = null;

        private bool isParameters = false;

        private bool isDeclSpec;

        private bool isParametersBraces = false;

        private bool isNameSpaced = false;

        private IList<Variable> OutVariable = new List<Variable>();
        public SimpleDeclarationContextVisitor(List<Variable> outVariables)
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
        //public override bool VisitBraceOrEqualInitializer([NotNull] CPP14Parser.BraceOrEqualInitializerContext context)
        //{
        //    var start = context.Start.Text;
        //    var stop = context.Stop.Text;
        //    return base.VisitBraceOrEqualInitializer(context);
        //}
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            return false;  // Декларация класса находится в ветке simpleDeclaration
        }
        public override bool VisitInitializer([NotNull] CPP14Parser.InitializerContext context)
        {
            //var expressionVisitor = new ExpressionVisitor();
            //Analyzer.Analyze(context,expressionVisitor);
            //VariableNames.AddRange(expressionVisitor.VariableNames);
            //CallFuncNames.AddRange(expressionVisitor.CallFuncNames);
            return false;
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
        private List<Variable> SelectParameters(IList<Parameter> parameters, IList<Variable> variables)
        {
            return parameters is null ? new List<Variable>() : 
                parameters.Where(p => p.Name is null && variables.Any(x=> x.Type == p.Type)).
                Select(x => new Variable {Name = x.Type}).ToList();
        }
        private bool IsDeclarationFunction(IList<Parameter> parameters,IList<Variable> variables)
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
                if (parameter.Name is null && variables.Any(v=> v.Name == parameter.Type))
                {
                    return false;
                }
            }
            return true;
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
            var name = context.GetTerminalNodes().FirstOrDefault()?.GetText();
            if (context.children.Count != 1 && name is not null) // если индификатор
            {
                return false;
            }

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

            var variable = new Variable()
            {
                Name = name,
                Type = DeclSpecifierSeqType
            };
            VariableNames.Add(variable);
            //VariableNames.AddRange(SelectParameters(Parameters, OutVariable));

            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
