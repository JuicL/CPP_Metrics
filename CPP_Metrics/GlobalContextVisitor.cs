using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics
{
    public class GlobalContextVisitor : CPP14ParserBaseVisitor<bool>
    {
        public GlobalContextVisitor(BaseContextElement contextElement)
        {
            ContextElement = contextElement;
        }

        public BaseContextElement ContextElement { get; }
        //Field current context
        public BaseContextElement Current { get; }
        public override bool VisitExpression([NotNull] CPP14Parser.ExpressionContext context)
        {
            var variableUsedVisitor = new UsedVariables(ContextElement);
            Analyzer.Analyze(context, variableUsedVisitor);

            foreach (var item in variableUsedVisitor.Identifiers)
            {
                var variable = ContextElement.GetVariableName(item);
                if (variable is not null)
                {
                    variable.References.Add(ContextElement);
                }
            }

            var typesUsedVisitor = new UsedClasses();
            Analyzer.Analyze(context, typesUsedVisitor);
            ContextElement.UsedClasses.AddRange(typesUsedVisitor.CPPTypes);
            return false;
        }
        public override bool VisitExpressionStatement([NotNull] CPP14Parser.ExpressionStatementContext context)
        {
            return true;
        }

        public override bool VisitCompoundStatement([NotNull] CPP14Parser.CompoundStatementContext context)
        {
            var statementSeq = context.statementSeq();
            if (statementSeq is not null)
            {
                var contextElem = new BaseContextElement();
                contextElem.Paren = ContextElement;
                var globalContextVisitor = new GlobalContextVisitor(contextElem);
                Analyzer.Analyze(statementSeq, globalContextVisitor);
            }
            return false;
        }

        public override bool VisitIterationStatement([NotNull] CPP14Parser.IterationStatementContext context)
        {
            var contextElem = new BaseContextElement();
            var forInitStat = context.forInitStatement();
            contextElem.Paren = ContextElement;
            if (forInitStat is not null)
            {

            }

            var compoundStatement = context.statement().compoundStatement()?.statementSeq();
            
            IParseTree statement = compoundStatement is not null ? compoundStatement : context.statement();
            if (statement is null) return false;
            var globalContextVisitor = new GlobalContextVisitor(contextElem);
            Analyzer.Analyze(statement, globalContextVisitor);

            return false;
        }
        // Statement // AND for 

        // Using = theTypeId
        public override bool VisitAliasDeclaration([NotNull] CPP14Parser.AliasDeclarationContext context)
        {
            var name = context.Identifier().GetText();
            if (context.theTypeId().abstractDeclarator() is not null)
                return false;
            var visitor = new TypeVisitor();
            var typeSpecifierSeq = context.theTypeId().typeSpecifierSeq();
            
            if (typeSpecifierSeq is null) return false;
            Analyzer.Analyze(typeSpecifierSeq, visitor);
            var assingType = visitor.Type;
            ContextElement.AliasDeclaration.TryAdd(name, assingType);
            return false;
        }

        // Using // Only Types
        public override bool VisitUsingDeclaration([NotNull] CPP14Parser.UsingDeclarationContext context)
        {
            SimpleUsing simpleUsing = new SimpleUsing();
            var unqualifiedId = context.unqualifiedId();

            if(unqualifiedId.Identifier() is not null)
            {
                simpleUsing.Name = unqualifiedId.Identifier().GetText();
            }
            else if(unqualifiedId.templateId() is not null)
            {
                var simpleTemplateId = unqualifiedId.templateId().simpleTemplateId();
                if(simpleTemplateId is not null)
                {
                    simpleUsing.Name = simpleTemplateId.templateName().Identifier().GetText();
                }
            }

            var nestedNameSpecifier = context.nestedNameSpecifier();
            if (nestedNameSpecifier != null)
            {
                var visitor = new NestedNameSpecifierVisitor();
                Analyzer.Analyze(nestedNameSpecifier, visitor);
                simpleUsing.Nested = visitor.NestedNames;
            }
            ContextElement.SimpleUsing.Add(simpleUsing);
            return false;
        }
        // Using namespace
        public override bool VisitUsingDirective([NotNull] CPP14Parser.UsingDirectiveContext context)
        {
            UsingNamespace usingNamespace = new();
            var namespaceName = context.namespaceName().children.First().GetChildren().First().GetText();
            usingNamespace.Name = namespaceName;
            var nested = context.nestedNameSpecifier();
            if(nested is not null)
            {
                var visitor = new NestedNameSpecifierVisitor();
                Analyzer.Analyze(nested, visitor);
                usingNamespace.Nested = visitor.NestedNames;
            }

            //var test = ContextElement.GetNameSpace(usingNamespace.Name, usingNamespace.Nested);

            ContextElement.UsingNamespaces.Add(usingNamespace);
            return false;
        }
        // Usings

        private List<CPPType> GetFullNameNamespace(BaseContextElement baseContextElement)
        {
            List<CPPType> nestedList = new();
            var currentContextElem = baseContextElement;
            while (currentContextElem is not null)
            {
                if (currentContextElem is NamespaceContext namespaceContext)
                {
                    nestedList.Add(new CPPType() { TypeName = namespaceContext.NameSpaceInfo.Name });
                }

                currentContextElem = currentContextElem.Paren;
            }
            return nestedList;
        }
        //NameSpace
        public override bool VisitNamespaceDefinition([NotNull] CPP14Parser.NamespaceDefinitionContext context)
        {
            if (ContextElement is not NamespaceContext)
                throw new Exception("Declaration namespace error");
           

            var namespaceInfo = context.GetNameSpaceInfo();
            namespaceInfo.Nested = GetFullNameNamespace(ContextElement);

            var declarationseq = context.declarationseq();
            NamespaceContext? namespaceContext;
            namespaceContext = ContextElement.GetNameSpace(namespaceInfo.Name);
            if (namespaceContext == null) // 
            {
                namespaceContext = new NamespaceContext
                {
                    NameSpaceInfo = namespaceInfo,
                    ParenNameSpace = (NamespaceContext)ContextElement,
                };
                namespaceContext.Paren = ContextElement;
                ContextElement.Children.Add(namespaceContext);
            }
            
            if(declarationseq is not null)
            {
                var globalContextVisitor = new GlobalContextVisitor(namespaceContext);
                Analyzer.Analyze(declarationseq, globalContextVisitor);
            }
            return false;
        }
        
        //Function or method
        public override bool VisitFunctionDefinition([NotNull] CPP14Parser.FunctionDefinitionContext context)
        {
            var funcVisitor = new FunctionDefinitionVisitor();
            Analyzer.Analyze(context, funcVisitor);
            
            if(ContextElement is ClassStructDeclaration classContext)
            {
                funcVisitor.FunctionInfo.NestedNames.Add(new CPPType() { TypeName = classContext.ClassStructInfo.Name });
            }

            var functionContext = new FunctionDeclaration();
            functionContext.FunctionInfo = funcVisitor.FunctionInfo;
            functionContext.Paren = ContextElement;

            foreach (var item in funcVisitor.FunctionInfo.Parameters.Where(x => x.Name is not null))
            {
                functionContext.VariableDeclaration.Add(item.Name, new Variable() { Name = item.Name ,Type = item.Type});
            }

            ContextElement.Children.Add(functionContext);

            if (!ContextElement.FunctionDeclaration.TryGetValue(funcVisitor.FunctionInfo.Name,out var functionInfos))
            {
                ContextElement.FunctionDeclaration.Add(funcVisitor.FunctionInfo.Name, new List<FunctionInfo>());
            }
            ContextElement.FunctionDeclaration[funcVisitor.FunctionInfo.Name].Add(funcVisitor.FunctionInfo);

            var globalContextVisitor = new GlobalContextVisitor(functionContext);
            var funcBody = (CPP14Parser.FunctionBodyContext)funcVisitor.FunctionInfo.FunctionBody;

            IParseTree funcStatement = funcBody.compoundStatement() is null ? funcBody : funcBody.compoundStatement().statementSeq();
            if (funcStatement == null) return false;
            Analyzer.Analyze(funcStatement, globalContextVisitor);
            return false;
        }

        // Variable declaration, function declaration
        public override bool VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context)
        {
            var visitor = new SimpleDeclarationContextVisitor(ContextElement);
            Analyzer.Analyze(context, visitor);

            foreach (var item in visitor.FunctionDeclaration)
            {
                if (!ContextElement.FunctionDeclaration.TryGetValue(item.Name, out var functionInfo))
                {
                    ContextElement.FunctionDeclaration.Add(item.Name, new List<FunctionInfo>());
                }
                ContextElement.FunctionDeclaration[item.Name].Add(item);
            }
            foreach (var item in visitor.VariablesDeclaration)
            {
                Variable value;
                if (ContextElement.VariableDeclaration.TryGetValue(item.Name, out value))
                {
                    return true;
                    //throw new Exception($"Переопределение имени {item.Name}");
                }
                ContextElement.VariableDeclaration.Add(item.Name, item);
            }
            return true;
        }
        private List<CPPType> GetNestedList(BaseContextElement baseContextElement)
        {
            List<CPPType> nestedList = new();
            var currentContextElem = baseContextElement;
            while (currentContextElem is not null)
            {
                if(currentContextElem is NamespaceContext namespaceContext)
                {
                    nestedList.Add(new CPPType() { TypeName = namespaceContext.NameSpaceInfo.Name });
                }
                else if(currentContextElem is ClassStructDeclaration classStruct)
                {
                    nestedList.Add(new CPPType() { TypeName = classStruct.ClassStructInfo.Name });
                }
                currentContextElem = currentContextElem.Paren;
            }
            return nestedList;
        }
        //Class
        public override bool VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            var visitor = new ClassStructVisitor();
            Analyzer.Analyze(context, visitor);

            List<CPPType> nestedList = GetNestedList(ContextElement);
            nestedList.Reverse();

            var classContext = new ClassStructDeclaration();
            classContext.ClassStructInfo = visitor.ClassStructInfo;
            classContext.ClassStructInfo.Nested = nestedList;

            foreach (var item in classContext.ClassStructInfo.BaseClasses)
            {
                var test = ContextElement.GetTypeName(item.TypeName,item.NestedNames);
            }

            classContext.Paren = ContextElement;
            ContextElement.Children.Add(classContext);
            if(classContext.ClassStructInfo.Name is null)
            {
                // Declaration variable type of structure, but without name
                classContext.ClassStructInfo.Name = "";
            }
            if (!ContextElement.TypeDeclaration.TryGetValue(classContext.ClassStructInfo.Name, out var classStructInfo))
            {
                ContextElement.TypeDeclaration.Add(classContext.ClassStructInfo.Name, classContext.ClassStructInfo);
            }
            else
            {
                //throw new Exception($"Переопределение класса {classStructInfo.Name}");
            }

            if(classContext.ClassStructInfo.Body is not null)
            {
                var memberSpecificationVisitor = new MemberSpecificationVisitor(classContext.ClassStructInfo.ClassKey, classContext);
                Analyzer.Analyze(classContext.ClassStructInfo.Body, memberSpecificationVisitor);

                foreach (var func in memberSpecificationVisitor.FunctionDefinition)
                {
                    var globalContextVisitor = new GlobalContextVisitor(classContext);
                    if (func.FunctionBody.Parent is null) continue;
                    Analyzer.Analyze(func.FunctionBody.Parent, globalContextVisitor);
                }
            }

            return false;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }

    }
}
