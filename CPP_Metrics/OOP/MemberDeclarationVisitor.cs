using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.OOP
{
    public class MemberDeclarationVisitor : CPP14ParserBaseVisitor<bool>
    {
        private ClassStructDeclaration ContextElement { get; }
        private AccesSpecifier AccesSpecifierSelector { get; }

        public List<FieldsInfo> VariablesDeclaration = new();
        
        public List<FunctionInfo> FunctionDeclaration = new();
        
        private CPPType? DeclSpecifierSeqType = null;

        private List<Parameter>? Parameters = null;

        private bool Override = false;

        private bool Final = false;

        private CPP14Parser.BraceOrEqualInitializerContext? BraceOrEqualInitializer;

        public MemberDeclarationVisitor(ClassStructDeclaration contextElement, AccesSpecifier accesSpecifierSelector)
        {
            ContextElement = contextElement;
            AccesSpecifierSelector = accesSpecifierSelector;
        }
        public override bool VisitMemberDeclarator([NotNull] CPP14Parser.MemberDeclaratorContext context)
        {
            var virtualSpecifierSeq = context.virtualSpecifierSeq();
            if(virtualSpecifierSeq != null)
            {
                foreach(var virtualSpecifier in virtualSpecifierSeq.virtualSpecifier())
                {
                    VisitVirtualSpecifier(virtualSpecifier);
                }
            }    
            var braceOrEqualInitializer = context.braceOrEqualInitializer();
            if(braceOrEqualInitializer != null)
            {
                BraceOrEqualInitializer = braceOrEqualInitializer;
            }
            return true;
        }
        public override bool VisitDeclSpecifierSeq([NotNull] CPP14Parser.DeclSpecifierSeqContext context)
        {
            var visitor = new TypeVisitor();
            Analyzer.Analyze(context, visitor);
            DeclSpecifierSeqType = visitor.Type;
            return false;
        }
        public new bool VisitVirtualSpecifier([NotNull] CPP14Parser.VirtualSpecifierContext context)
        {
            var virtualSpecifier = context.children.First().GetText();
            if(virtualSpecifier.Equals("override"))
            {
                Override = true;
            }   
            else if(virtualSpecifier.Equals("final"))
            {
                Final = true;
            }

            return false;
        }
        public override bool VisitNoPointerDeclarator([NotNull] CPP14Parser.NoPointerDeclaratorContext context)
        {
            var parameters = context.parametersAndQualifiers();
            if (parameters is not null)
            {
                VisitParametersAndQualifiers(parameters);
            }
            return true;
        }
        public new bool VisitParametersAndQualifiers([NotNull] CPP14Parser.ParametersAndQualifiersContext context)
        {
            Parameters = new List<Parameter>();
            var parameterDeclaration = context.children.FirstOrDefault(x => x is CPP14Parser.ParameterDeclarationClauseContext)
                ?.GetChildren()
                .FirstOrDefault(x => x is CPP14Parser.ParameterDeclarationListContext)
                ?.GetChildren()
                .Where(x => x is CPP14Parser.ParameterDeclarationContext).ToList();
            if(parameterDeclaration is null) return false;
            foreach (var item in parameterDeclaration)
            {
                var parameterVisitor = new ParameterVisitor();
                Analyzer.Analyze(item, parameterVisitor);
                Parameters.Add(parameterVisitor.Parameter);
            }
            return false;
        }
        private bool IsDeclarationFunction(IList<Parameter>? parameters)
        {
            if (parameters is null) return false; // отсутствие параметров
            if (parameters.Count == 0) return true;//пустые скобки

            foreach (var parameter in parameters)
            {
                // если имя и тип не нулевые значит точно декларация
                if (parameter.Name is not null && parameter.Type is not null)
                {
                    return true;
                }
                if (parameter.Name is null)
                {
                    //Поле класса
                    if (ContextElement.ClassStructInfo.Fields.Any(f => f.Name == parameter.Type.TypeName)
                        || VariablesDeclaration.Any(f => f.Name == parameter.Type.TypeName))
                    {
                        return false;
                    }
                    // имя null проверяем type на равенство с именем переменной
                    if (ContextElement.GetVariableName(parameter.Type.TypeName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void ParseDescructor([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            var className = context.className();
            if (className is not null)
            {
                var identifier = className.Identifier();
                if(identifier is not null) //
                {
                    var functionInfo = new FunctionInfo
                    {
                        Name = "~" + identifier.GetText(),
                        AccesSpecifier = AccesSpecifierSelector,
                        Override = Override,
                        Final = Final,
                        IsPure = IsPure(BraceOrEqualInitializer)
                    };
                    FunctionDeclaration.Add(functionInfo);
                }
                //SimpleTemplateId
            }
            //DeclType
        }
        private bool IsPure([NotNull] CPP14Parser.BraceOrEqualInitializerContext? context)
        {
            if (context is null) return false;

            var init = context.initializerClause();
            if(init is not null)
            {
                if(String.Equals(init.GetText(),"0"))
                { return true; }
            }
            return false;
        }
        public override bool VisitUnqualifiedId([NotNull] CPP14Parser.UnqualifiedIdContext context)
        {
            //Деструктор
            var tilde = context.Tilde();
            if (tilde is not null)
            {
                ParseDescructor(context);
                return false;
            }

            var identifier = context.Identifier();
            if (identifier is null) return true;

            var name = identifier.GetText();

            // Конструктор
            if (DeclSpecifierSeqType is null && Parameters is not null)
            {
                var functionInfo = new FunctionInfo
                {
                    Name = name,
                    AccesSpecifier = AccesSpecifierSelector,
                    Override = Override,
                    Final = Final,
                    
                };
                FunctionDeclaration.Add(functionInfo);
                return false;
            }

            // проверка декларация функции по параметрам внутри parametrsBraced
            if (DeclSpecifierSeqType is not null && IsDeclarationFunction(Parameters))
            {
                var functionInfo = new FunctionInfo
                {
                    Name = name,
                    AccesSpecifier = AccesSpecifierSelector,
                    ReturnType = DeclSpecifierSeqType,
                    Override = Override,
                    Final = Final,
                    IsPure = IsPure(BraceOrEqualInitializer),
                };
                FunctionDeclaration.Add(functionInfo);

                return false;
            }


            var variable = new FieldsInfo()
            {
                Name = name,
                Type = DeclSpecifierSeqType,
                AccesSpecifier = AccesSpecifierSelector
            };
            VariablesDeclaration.Add(variable);

            return true;
        }
        public override bool VisitChildren(IRuleNode node)
        {
            return true;
        }


    }
}
