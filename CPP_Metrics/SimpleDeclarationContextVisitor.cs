

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;

namespace CPP_Metrics
{
    /// <summary>
    /// парсинг SimpleDeclaration...
    /// </summary>
    /*
     Рассмотрены случа объявления переменной и функции(с указанием namespace). Переменная типа класса. Присваивание переменной(классу) функции/переменной
     */
    public class UnqualifiedId
    {
        public Identifier Identifier { get; set; }
    }
    public class QualifiedId : UnqualifiedId
    {
        public IList<Identifier> Nested { get; set; }
    }
 

    public class Context
    {
        public IList<string> UserTypes = new List<string>();

        public List<Variable> outVariables = new();
        
        public List<string> DeclFuncNames = new();
    }

    public class FunctionDeclaration
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public IList<Parameter> Parameters { get; set; }    

    }
    public class SimpleDeclarationContextVisitor : CPP14ParserBaseVisitor<bool>
    {
        

        public List<string> CallFuncNames = new();


        private List<Parameter>? Parameters = null;


        public string? DeclSpecifierSeqType = null; // Тип

        
        private bool isNameSpaced = false;

        private bool? NoPointerBrace;

        private Context Context;
        public SimpleDeclarationContextVisitor(Context context)
        {
            Context = context;
        }
        public SimpleDeclarationContextVisitor()
        {
        }
        
        private List<Variable> SelectParameters(IList<Parameter> parameters, IList<Variable> variables)
        {
            return parameters is null ? new List<Variable>() :
                parameters.Where(p => p.Name is null && variables.Any(x => x.Type == p.Type)).
                Select(x => new Variable { Name = x.Type }).ToList();
        }
        private bool IsDeclarationFunction(IList<Parameter>? parameters, IList<Variable> variables)
        {
            if(parameters is null) return false; // отсутствие параметров
            if(parameters.Count == 0) return true;//пустые скобки

            foreach (var parameter in parameters)
            {
                // если имя и тип не нулевые значит точно декларация
                if (parameter.Name is not null && parameter.Type is not null)
                {
                    return true;
                }
                // имя null проверяем type на равенство с именем переменной
                if (parameter.Name is null && variables.Any(v => v.Name == parameter.Type))
                {
                    return false;
                }
            }
            return true;
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
            Parameters = new List<Parameter>();
            //isParameters = true;
            var parameterDeclaration = context.children.FirstOrDefault(x => x is CPP14Parser.ParameterDeclarationClauseContext)
                ?.GetChildren()
                .FirstOrDefault(x => x is CPP14Parser.ParameterDeclarationListContext)
                ?.GetChildren()
                .Where(x => x is CPP14Parser.ParameterDeclarationContext).ToList();
            if (parameterDeclaration is null) return false;
            foreach (var item in parameterDeclaration)
            {
                var parameterVisitor = new ParameterVisitor();
                Analyzer.Analyze(item, parameterVisitor);
                Parameters.Add(parameterVisitor.Parameter);
            }
            return false;
        }
        public override bool VisitDeclSpecifierSeq([NotNull] CPP14Parser.DeclSpecifierSeqContext context)
        {
            //this.isDeclSpec = true;
            var visitor = new TypeVisitor();
            Analyzer.Analyze(context, visitor);
            if(visitor.Type.Length > 0)
            {
                DeclSpecifierSeqType = visitor.Type;
            }
            return false;
        }
        public override bool VisitDeclarator([NotNull] CPP14Parser.DeclaratorContext context)
        {
            var poinerDecl = context.pointerDeclarator();
            var noPointerDecl = poinerDecl.noPointerDeclarator();
            // Если сразу после имени идут скобки (имя будет находиться в DeclType)
            
            NoPointerBrace = poinerDecl?.pointerOperator(0) is not null || 
                                noPointerDecl?.pointerDeclarator() is null ? false : true;
            return true;   
        }
        public override bool VisitNoPointerDeclarator([NotNull] CPP14Parser.NoPointerDeclaratorContext context)
        {
            // Случай, когда после имени стоят ( )
            var parameters = context.parametersAndQualifiers();
            if (parameters is null)
                return true;
            VisitParametersAndQualifiers(parameters);

            //isParametersBraces = context.children.Count > 1 ? true : false;
            
            return true;
        }
        public override bool VisitIdExpression([NotNull] CPP14Parser.IdExpressionContext context)
        {
            // Случай,когда перед именем стоит <::>
            isNameSpaced = context.children.First() is CPP14Parser.QualifiedIdContext;
            
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
            // Если nested имеет только :: -> перекинуть тип из declType в nested, обнулиtь declType
            // Если NoPointer имеет () значит проверить является ли declType функцией или типом, и проверить параметр в скобках
            // !! Проверка Типов!!! Для декларации функций расширить проверку типов вход. параметров.

            // Initializer(нет declType,проверить текущее имя) // Parameters // NoPointer

            var identifier = context.Identifier();
            if (identifier is null) return true;
            var name = identifier.GetText();

            // Декларация переменной в стиле конструктора или вызов функции с 1 переменной
            if(NoPointerBrace == true && DeclSpecifierSeqType is not null)
            {
                if (Context.DeclFuncNames.Contains(DeclSpecifierSeqType)) // проверка не являтеся ли типом
                {
                    CallFuncNames.Add(DeclSpecifierSeqType);
                    Context.outVariables.Add(new Variable() { Name = name });
                    return false;
                }
            }

            // проверка декларация функции по параметрам внутри parametrsBraced
            if(DeclSpecifierSeqType is not null && IsDeclarationFunction(Parameters, Context.outVariables))
            {
                Context.DeclFuncNames.Add(name);
                return false;
            }

            //if (isNameSpaced && isParametersBraces || !isDeclSpec && isParametersBraces)// Если :: и () //() без declSpec -- функция
            //{
            //    CallFuncNames.Add(name);
            //    return false;
            //}

            var variable = new Variable()
            {
                Name = name,
                Type = DeclSpecifierSeqType
            };
            Context.outVariables.Add(variable);

            //VariableNames.AddRange(SelectParameters(Parameters, OutVariable));

            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
