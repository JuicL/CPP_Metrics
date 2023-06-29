using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ANTLR4Tools;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using static CPP14Parser;

namespace CPP_Metrics.CyclomaticComplexity
{
    /*
        Привязка последнего добавленного блока с головой графа(при первом проходе) не предусматривается в визиторе, это происходит на уровне выше,т.е. \
        в классе запуска анализа цикломатической сложности

     */
  
    public class CyclomaticComplexityVisitor : CPP14ParserBaseVisitor<bool>
    {
        public CyclomaticComplexityVisitor(CyclomaticGraph graph, CyclomaticVertex? from, CyclomaticVertex? to)
        {
            Graph = graph;
            To = (to is null ? Graph.Tail : to);
            From = from;
        }

        public CyclomaticVertex Vertex { get; private set; }
        public CyclomaticGraph Graph { get; }
        public CyclomaticVertex To { get; }
        /// <summary>
        /// Используется в привязке case-ов в switch
        /// </summary>
        public CyclomaticVertex? From { get; }
        public CyclomaticVertex? Last { get; private set; } = null;

        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }
       
        private void ParseStatementContext(ParserRuleContext context)
        {
            //var statement = new Vertex(Type.Statment);
            //if (Last is null)
            //{
            //    Graph.Edges.Add(new Edge(statement, To));
            //}
            //else
            //{
            //    if (Last.Type != Type.Statment)
            //        Graph.Edges.Add(new Edge(statement, Last));
            //}
        }
        
        private void ParseWhileStatement(IterationStatementContext context)
        {
            var whileVertex = Graph.CreateVertex();
            whileVertex.Type = Type.While;
            
            var condition = context.condition();
            if (condition is not null)
            {
                ConditionCyclomaticVisitor v = new();
                Analyzer.Analyze(condition, v);
                whileVertex.Value += v.CountLogicalExpression;
            }
            //var endWhileVertex = new Vertex(Type.EmpyStatement);
            //Graph.Vertices.Add(endWhileVertex);
            //Graph.Edges.Add(new Edge(whileVertex, endWhileVertex)); // False way from while to next state

            var afterEndVertex = Last is null ? To : Last;// Путь от endWhileVertex до последнего добавленног или хвосту
            ConntectLastAddedWithCurrentVertex(whileVertex, afterEndVertex); // Связка вниз

            var visitor = new CyclomaticComplexityVisitor(Graph, whileVertex, whileVertex);
            Analyzer.AnalyzeR(context.children, visitor);

            //True way
            ConntectLastAddedWithCurrentVertex(whileVertex, visitor.Last); // Связка наверх
            
            if (Graph.Edges.Where(x => x.From.Id == whileVertex.Id).Count() == 1) 
            {
                var whileContext = Graph.CreateVertex();
                whileContext.Type = Type.EmpyStatement;
                Graph.CreateEdge(whileVertex, whileContext);
                Graph.CreateEdge(whileContext, whileVertex);
            }
            Last = whileVertex;
        }
        /// <summary>
        /// 
        ///_____If LeftParen condition RightParen statement (Else statement)?
        /// <param name="lastAdded"></param>
        /// <param name="current"></param>
        /// TODO : убрать Узел Context
        private void ParseIfSection([NotNull] CPP14Parser.SelectionStatementContext context)
        {
            //Создать вершину if
            var ifVertex = Graph.CreateVertex();
            ifVertex.Type = Type.If;
            var condition = context.condition();
            if(condition is not null)
            {
                ConditionCyclomaticVisitor v = new();
                Analyzer.Analyze(condition, v);
                ifVertex.Value += v.CountLogicalExpression;
            }

            var ifStatement = Graph.CreateVertex(); // Привязка след дочерних узлов к нему
            ifStatement.Type = Type.EmpyStatement;
            ifStatement.Name = "ifStat";
            Graph.CreateEdge(ifVertex, ifStatement);// Связь от if до ifStatement

            var endIf = Graph.CreateVertex();
            endIf.Type = Type.EmpyStatement;
            endIf.Name = "endIf";
            //(2) endIf = Last is null ? To : Last;
            CyclomaticComplexityVisitor visitor = new CyclomaticComplexityVisitor(Graph, ifStatement, endIf);
            //TODO : От узла стейтмента
            
            Analyzer.AnalyzeR(context.statement().First(), visitor);

            ConntectLastAddedWithCurrentVertex(ifStatement, visitor.Last);// Привязка вверх

            // Если у стейтмента нет дочерних узлов, значит выход идет к последнему добавленному или к хвосту// Привязка вниз
            if (Graph.Edges.FirstOrDefault(x => x.From.Id == ifStatement.Id) is null) // think -- проверять у ifVertex и выше привязывать к нему
            {
                Graph.CreateEdge(ifStatement, endIf);
            }
            // Закомментить если внести (2)
            var afterEndIfVertex = Last is null ? To : Last;
            Graph.CreateEdge(endIf, afterEndIfVertex);


            // Проверить есть ли вершина else
            var isElse = context.Else();
            if (isElse is not null)
            {
                var elseVertex = Graph.CreateVertex();
                elseVertex.Type = Type.Else;
                Graph.CreateEdge(ifVertex, elseVertex);

                CyclomaticComplexityVisitor elseVisitor = new CyclomaticComplexityVisitor(Graph, elseVertex, endIf);
                Analyzer.AnalyzeR(context.statement().Last(), elseVisitor);

                ConntectLastAddedWithCurrentVertex(elseVertex, elseVisitor.Last);// Привязка вверх

                if (Graph.Edges.FirstOrDefault(x => x.From.Id == elseVertex.Id) is null)
                {
                    Graph.CreateEdge(elseVertex, endIf);
                }
            }
            else
            {
                // Свзять If с endIf
                Graph.CreateEdge(ifVertex, endIf);
            }
            Last = ifVertex; // Последний добавленный верховный узел( не еlse т.к он встанет ниже иф-а)

        }
        
        private void ParseDoWhileStatement(IterationStatementContext context)
        {
            var doVertex = Graph.CreateVertex();
            doVertex.Type = Type.Do;

            var condition = context.condition();
            if (condition is not null)
            {
                ConditionCyclomaticVisitor v = new();
                Analyzer.Analyze(condition, v);
                doVertex.Value += v.CountLogicalExpression;
            }

            var whileVertex = Graph.CreateVertex();
            whileVertex.Type = Type.While;

            Graph.CreateEdge(whileVertex, doVertex);

            var afterWhile = Last is null ? To : Last;
            ConntectLastAddedWithCurrentVertex(whileVertex, afterWhile); // Делаем связку пораньше, чтобы обработать break

            var visitor = new CyclomaticComplexityVisitor(Graph, doVertex, whileVertex);
            Analyzer.AnalyzeR(context.children, visitor);

            ConntectLastAddedWithCurrentVertex(doVertex, visitor.Last);
            if(Graph.Edges.FirstOrDefault(x => x.From.Id == doVertex.Id) is null)
            {
                Graph.CreateEdge(doVertex, whileVertex);
                //var doWhileContext = Graph.CreateVertex();
                //doWhileContext.Type = Type.EmpyStatement;
                //Graph.CreateEdge(doVertex, doWhileContext);
                //Graph.CreateEdge(doWhileContext, whileVertex);
            }
            

            Last = doVertex;
        }
        // TODO : do
        private void ParseForStatement(IterationStatementContext context)
        {
            var declarationVertex = Graph.CreateVertex();
            declarationVertex.Type = Type.Statment;
            
            var forVertex = Graph.CreateVertex();
            forVertex.Type = Type.For;
            Graph.CreateEdge(declarationVertex, forVertex);

            var condition = context.condition();
            if (condition is not null)
            {
                ConditionCyclomaticVisitor v = new();
                Analyzer.Analyze(condition, v);
                forVertex.Value += v.CountLogicalExpression;
            }

            var iterationVertex = Graph.CreateVertex();
            iterationVertex.Type = Type.Statment;
            Graph.CreateEdge(iterationVertex, forVertex);


            Graph.CreateEdge(forVertex, Last is null ? To : Last);

            var visitor = new CyclomaticComplexityVisitor(Graph, forVertex, iterationVertex);
            Analyzer.AnalyzeR(context.children, visitor);

            ConntectLastAddedWithCurrentVertex(forVertex, visitor.Last);
            if(Graph.Edges.FirstOrDefault(x => x.From.Id == forVertex.Id) is null)
            {
                var contextVertex = Graph.CreateVertex();
                contextVertex.Type = Type.Statment;
                Graph.CreateEdge(forVertex, contextVertex);
                Graph.CreateEdge(contextVertex, iterationVertex);

            }
            Last = declarationVertex;
        }
        // TODO : do
        private void ParseSwitchStatement(SelectionStatementContext context)
        {
            var switchVertex = Graph.CreateVertex();
            switchVertex.Type = Type.Switch;
            var condition = context.condition();
            if (condition is not null)
            {
                ConditionCyclomaticVisitor v = new();
                Analyzer.Analyze(condition, v);
                switchVertex.Value += v.CountLogicalExpression;
            }
            var endSwitchVertex = Graph.CreateVertex();
            endSwitchVertex.Type = Type.EmpyStatement;

            Graph.CreateEdge(switchVertex, endSwitchVertex);// Пустой switch дает 1 цикломатическую сложность

            var visitor = new CyclomaticComplexityVisitor(Graph, switchVertex, endSwitchVertex);
            Analyzer.AnalyzeR(context.children, visitor);
            Graph.CreateEdge(endSwitchVertex, Last is null ? To : Last);

            Last = switchVertex;
        }

        private void ParseCaseStatement(LabeledStatementContext context)
        {
            var caseSwitch = context.Case();
            var defaultLabel = context.Default();
            if (caseSwitch is null && defaultLabel is null) return;
            //var caseVertexContext = caseSwitch is null ? defaultLabel : caseSwitch;

            var caseVertex = Graph.CreateVertex();
            caseVertex.Type = Type.Case;
            Graph.CreateEdge(From, caseVertex);

            var visitor = new CyclomaticComplexityVisitor(Graph, caseVertex, To);
            Analyzer.AnalyzeR(context.children, visitor);

            ConntectLastAddedWithCurrentVertex(caseVertex, visitor.Last);
            
            if (Graph.Edges.SingleOrDefault(x => x.From.Id == caseVertex.Id) is null)
            {
                Graph.CreateEdge(caseVertex, To);
            }

        }
        
        private void ConntectLastAddedWithCurrentVertex(CyclomaticVertex current, CyclomaticVertex? lastAdded)
        {
            if (lastAdded is null) return;
            
            Graph.CreateEdge(current, lastAdded);
        }
        
        /// <summary>
        /// Тернарный If
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool VisitConditionalExpression([NotNull] ConditionalExpressionContext context)
        {
            if (context.Question() is null)
                return true;

            // Expression(true) assigmentExpression(false)
            var ifVertex = Graph.CreateVertex();
            ifVertex.Type = Type.If;

            var trueStat = Graph.CreateVertex();
            trueStat.Type = Type.EmpyStatement;
            Graph.CreateEdge(ifVertex, trueStat);// Связь от if до ifStatement

            var condition = context.logicalOrExpression();
            if (condition is not null)
            {
                ConditionCyclomaticVisitor v = new();
                Analyzer.Analyze(condition, v);
                ifVertex.Value += v.CountLogicalExpression;
            }

            var falseStat = Graph.CreateVertex();
            falseStat.Type = Type.EmpyStatement;
            Graph.CreateEdge(ifVertex, falseStat);// Связь от if до ifStatement


            var endIf = Graph.CreateVertex();
            endIf.Type = Type.EmpyStatement;
            endIf.Name = "endIf";
            Graph.CreateEdge(trueStat, endIf);
            Graph.CreateEdge(falseStat, endIf);

            var afterEndIfVertex = Last is null ? To : Last;
            Graph.CreateEdge(endIf, afterEndIfVertex);

            
            Last = ifVertex; // Последний добавленный верховный узел( не еlse т.к он встанет ниже иф-а)

            return false;
        }
        public override bool VisitSelectionStatement([NotNull] SelectionStatementContext context)
        {
            if(context.If() is not null)
            {
                ParseIfSection(context);
            }
            else if(context.Switch() is not null)
            {
                ParseSwitchStatement(context);
            }
            return false;
        }
        public override bool VisitLabeledStatement([NotNull] LabeledStatementContext context)
        {
            ParseCaseStatement(context);
            return false;
        }
        public override bool VisitIterationStatement([NotNull] IterationStatementContext context)
        {
            var iterationType = context.GetChild(0).GetText();
            if(iterationType == "while")
            {
                ParseWhileStatement(context);
            }
            else if(iterationType == "do")
            {
                ParseDoWhileStatement(context);
            }
            else if(iterationType == "for")
            {
                ParseForStatement(context);
            }

            return false;
        }
        public override bool VisitJumpStatement([NotNull] JumpStatementContext context)
        {
            var jumpType = context.GetTerminalNodes().First().GetText();
            if (jumpType == "break")
            {
                var jumpVertex = Graph.CreateVertex();
                jumpVertex.Type = Type.Jump;
                var breakOut = Graph[From];
                Graph.CreateEdge(jumpVertex, breakOut.FirstOrDefault() is not null ? breakOut.First() : To); // Куда выходим
                Last = jumpVertex;

            }
            else if (jumpType == "return")
            {
                var returnVertex = Graph.CreateVertex();
                returnVertex.Type = Type.Jump;
                Graph.CreateEdge(returnVertex, Graph.Tail);
                Last = returnVertex;
                // Возвращаемый тип может быть тернарным оператором
            }
            else if(jumpType == "goto")
            { //============================
            }
            else if(jumpType == "continue")
            {
                var continueVertex = Graph.CreateVertex();
                continueVertex.Type = Type.Jump;
                Graph.CreateEdge(continueVertex, From);
                Last = continueVertex;
            }
            return true;
        }

    }
}
