

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
    /// <summary>
    /// парсинг SimpleDeclaration...
    /// </summary>
    /*
     Рассмотрены случа объявления переменной и функции(с указанием namespace). Переменная типа класса. Присваивание переменной(классу) функции/переменной
     */
  

    public class SimpleDeclarationContextVisitor : CPP14ParserBaseVisitor<bool>
    {
        
        public List<string> CallFuncNames = new();

        public List<Variable> VariablesDeclaration = new();

        public List<FunctionInfo> FunctionDeclaration = new();

        private List<Parameter>? Parameters = null;

        private List<CPPType>? NestedNames;

        public CPPType? DeclSpecifierSeqType = null; // Тип
        
        private bool isNameSpaced = false;


        private bool? NoPointerBrace;

        public BaseContextElement ContextElement { get; }

        public SimpleDeclarationContextVisitor(BaseContextElement contextElement)
        {
            ContextElement = contextElement;
        }

        public SimpleDeclarationContextVisitor()
        {

        }
        
        
        //private List<Variable> SelectParameters(IList<Parameter> parameters, IList<Variable> variables)
        //{
        //    return parameters is null ? new List<Variable>() :
        //        parameters.Where(p => p.Name is null && variables.Any(x => x.Type == p.Type)).
        //        Select(x => new Variable { Name = x.Type }).ToList();
        //}

        private bool IsDeclarationFunction(IList<Parameter>? parameters)
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
                if (parameter.Name is null 
                    && ContextElement.GetVariableName(parameter.Type.TypeName))
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
            var visitor = new TypeVisitor();
            Analyzer.Analyze(context, visitor);
            DeclSpecifierSeqType = visitor.Type;
            
            return false;
        }
        public override bool VisitDeclarator([NotNull] CPP14Parser.DeclaratorContext context)
        {
            var poinerDecl = context.pointerDeclarator();
            var noPointerDecl = poinerDecl.noPointerDeclarator();
            // Если сразу после имени идут скобки (имя будет находиться в DeclType)
            
            NoPointerBrace = poinerDecl?.pointerOperator(0) is not null || 
                                noPointerDecl?.pointerDeclarator() is null ? false : true;
            var parametersAndQualifiers = noPointerDecl.parametersAndQualifiers();
            if (parametersAndQualifiers is null)
            {
                Parameters = null;
            }
            else
                VisitParametersAndQualifiers(parametersAndQualifiers);

            return true;   
        }
        public override bool VisitNestedNameSpecifier([NotNull] CPP14Parser.NestedNameSpecifierContext context)
        {
            var visitor = new NestedNameSpecifierVisitor();
            Analyzer.Analyze(context, visitor);
            NestedNames = visitor.NestedNames;
            return false;
        }
        public override bool VisitNoPointerDeclarator([NotNull] CPP14Parser.NoPointerDeclaratorContext context)
        {
            //// Случай, когда после имени стоят ( )
            //var parameters = context.parametersAndQualifiers();
            //if (parameters is null)
            //    return true;
            //VisitParametersAndQualifiers(parameters);

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
         *        
        Identifier
        | operatorFunctionId
        | conversionFunctionId
        | literalOperatorId
        | Tilde (className | decltypeSpecifier)
        | templateId;
        */
        public override bool VisitOperatorFunctionId([NotNull] CPP14Parser.OperatorFunctionIdContext context)
        {
            var name = context.children.First().GetText() // operator
                + context.children.Last().GetTerminalNodes().First().GetText(); // theOperator 
            var functionInfo = new FunctionInfo
            {
                Name = name,
                ReturnType = DeclSpecifierSeqType,
                Parameters = Parameters,
            };
            FunctionDeclaration.Add(functionInfo);

            return base.VisitOperatorFunctionId(context);
        }
        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            

            // !! Проверка Типов!!! Для декларации функций расширить проверку типов вход. параметров.
            // Initializer(нет declType,проверить текущее имя) // Parameters // NoPointer

            var identifier = context.Identifier();
            if (identifier is null) return true;
            var name = identifier.GetText();

            // Если nested имеет только :: -> перекинуть тип из declType в nested, обнулиtь declType
            if (NestedNames is not null && NestedNames.Count == 0)
            {
                if (DeclSpecifierSeqType is not null)
                {
                    NestedNames = DeclSpecifierSeqType.NestedNames;
                    DeclSpecifierSeqType.NestedNames = null;
                    NestedNames.Add(DeclSpecifierSeqType);
                }
            }
            
            // Если NoPointer имеет () значит проверить является ли declType функцией или типом, и проверить параметр в скобках
            //Вызов функции с 1 переменной (По причине схожести с декларации переменной в стиле конструктора)
            if (NoPointerBrace == true && DeclSpecifierSeqType is not null)
            {
                if (ContextElement.GetFunctionName(DeclSpecifierSeqType.TypeName)) //TODO: проверка не являтеся ли типом
                {
                    CallFuncNames.Add(DeclSpecifierSeqType.TypeName);
                    Console.WriteLine($"#FunCall# {DeclSpecifierSeqType.TypeName}");
                    //Context.outVariables.Add(new Variable() { Name = name });
                    return false;
                }
            }
            if(DeclSpecifierSeqType is null && Parameters is not null)
            {
                Console.WriteLine($"#FunCall# {name}");
                return false;
            }
            // проверка декларация функции по параметрам внутри parametrsBraced
            if(DeclSpecifierSeqType is not null && IsDeclarationFunction(Parameters))
            {
                var functionInfo = new FunctionInfo
                {
                    Name = name,
                    ReturnType = DeclSpecifierSeqType,
                    Parameters = Parameters is null ? new List<Parameter>() : Parameters,

                };
                FunctionDeclaration.Add(functionInfo);
                return false;
            }

            var variable = new Variable()
            {
                Name = name,
                Type = DeclSpecifierSeqType
            };
            VariablesDeclaration.Add(variable);
            

            //VariableNames.AddRange(SelectParameters(Parameters, OutVariable));

            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
/*//Конструктор
            if(DeclSpecifierSeqType is null && NestedNames is not null)
            {
                if(NestedNames.Last().TypeName.Equals(name))
                {
                    var functionInfo = new FunctionInfo
                    {
                        Name = name,
                        ReturnType = DeclSpecifierSeqType,
                        Parameters = Parameters is null ? new List<Parameter>() : Parameters,

                    };
                    FunctionDeclaration.Add(functionInfo);
                }
            }
 */