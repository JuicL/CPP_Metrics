using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace CPP_Metrics
{
    public class VariableVisitor : CPP14ParserBaseVisitor<bool>
    {
        //TODO:  Собрать только имена переменных(всех) пока что без типа
        // Как обрабатывать(для метрики) переменные типа класса
        // initDeclaratorList присваивание
        // Функция это () и :: NOPointer < NestedName  > ()
        public List<string> VariableNames = new List<string>();
        public List<string> FuncNames = new List<string>();

        private bool isDecl = false;
        private void CollectionNames(IParseTree context)
        {
            var visitor = new SimpleDeclarationContextVisitor(isDecl);
            Analyzer.Analyze(context, visitor);
            VariableNames.AddRange(visitor.VariableNames);
            FuncNames.AddRange(visitor.CallFuncNames);
        }

        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            var funcBody = context.children.FirstOrDefault(x => x is CPP14Parser.FunctionBodyContext);
            var declarator = context.children.FirstOrDefault(x => x is CPP14Parser.DeclaratorContext);
            
            //CollectionNames(declarator);

            var visitor = new VariableVisitor();
            Analyzer.Analyze(funcBody, visitor);
            VariableNames.AddRange(visitor.VariableNames);
            FuncNames.AddRange(visitor.FuncNames);

            //var visitor1 = new FunctionDefinitionVisitor();
            //Analyzer.Analyze(context, visitor1);
            
            return false;
        }

        public override bool VisitExpressionStatement([NotNull] CPP14Parser.ExpressionStatementContext context)
        {
            CollectionNames(context);
            return true;
        }

        public override bool VisitIterationStatement([NotNull] CPP14Parser.IterationStatementContext context)
        {
            var controlVariables = new List<string>();
            // Управляющие переменные для for/while ets.
            var iterationChildWhoutStatenemt = context.children.Where(x => x is not CPP14Parser.StatementContext);
            foreach (var item in iterationChildWhoutStatenemt)
            {
                var visitor = new SimpleDeclarationContextVisitor();
                Analyzer.Analyze(item, visitor);
                controlVariables.AddRange(visitor.VariableNames);
            }
            VariableNames.AddRange(controlVariables);

            var statement = context.children.FirstOrDefault(x => x is CPP14Parser.StatementContext);
            if(statement != null)// Тело циклических блоков 
            {
                var visitor = new VariableVisitor();
                Analyzer.Analyze(statement, visitor);
                VariableNames.AddRange(visitor.VariableNames);
            }
            return false;
        }

        public override bool VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context)
        {
            var declSpecifire = context.children.FirstOrDefault(x => x is CPP14Parser.DeclSpecifierSeqContext);
            if (declSpecifire != null) // Тип или namespase
            {
                //TODO : Разбор типа переменной
                isDecl = true;
            }
            else
            {
                isDecl = false;
            }

            return true;
        }

        // TODO: переделать на парсинг InitDeclaratorListContext
        public override bool VisitInitDeclarator([NotNull] CPP14Parser.InitDeclaratorContext context)
        {
            //TODO: явный тип а не первы и последний
            var declarator = context.children.First();
            CollectionNames(declarator);
            if(context.children.Count > 1)
            {
                var initializer = context.children.Last();
                CollectionNames(initializer);
            }

            return true;
        }
        
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
    }
}
