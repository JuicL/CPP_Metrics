using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.Metrics
{
    public class CBOGraph: Graph<CBOVertex,CBOEdge>
    {

    }

    public class CBOEdge : IEdge<CBOVertex>
    {
        public CBOVertex From { get ; set ; }
        public CBOVertex To { get; set; }
        public decimal Price { get; set; }
    }
    public class CBOVertex : IVertex
    {
        public Guid Id { get; set; }
        // Полный путь до типа
        public string FullName { get; set; }
        public string? Namespace { get; set; }
    }

    public class CBOValue
    {
        public CPPType Type { get; set; }
        public int Value { get; set; }
    }

    public class CBOMetric : IMetric
    {
        public IReportBuilder ReportBuilder { get ; set ; }
        public Dictionary<string, CBOVertex> Classes { get; set; } = new();
        public CBOGraph Graph { get; set; } = new();
        public List<MetricMessage> Messages { get; set; } = new ();

        private void ConnectType(CPPType? type,CBOVertex from,ClassStructDeclaration classItem)
        {
            if (type == null) return;
            var fieldType = classItem.GetTypeName(type.TypeName, type.NestedNames);
            if (fieldType != null && fieldType is ClassStructDeclaration classStructDecl)
            {
                var classStructInfoForField = classStructDecl.ClassStructInfo;
                if (!Classes.TryGetValue(classStructInfoForField.GetFullName(), out var cBOVertexForField))
                {
                    cBOVertexForField = Graph.CreateVertex();
                    cBOVertexForField.FullName = classStructInfoForField.GetFullName();
                    cBOVertexForField.Namespace = classStructInfoForField.GetNamespace();
                    Classes.Add(classStructInfoForField.GetFullName(), cBOVertexForField);
                }
                // Тип поля класса является другой класс, связываем
                Graph.CreateEdge(from, cBOVertexForField);
            }
            else
            {
                // Если тип класса явлется внешним по отношению к файлам проекта
                var fullName = type.GetFullName();
                if (!Classes.TryGetValue(fullName, out var cBOVertexOutContext))
                {
                    cBOVertexOutContext = Graph.CreateVertex();
                    cBOVertexOutContext.FullName = fullName;
                    Classes.Add(fullName, cBOVertexOutContext);
                }
                Graph.CreateEdge(from, cBOVertexOutContext);
            }
        }

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {

            // Создать контекст
            BaseContextElement.Clear();
            BaseContextElement contextElement = BaseContextElement.GetGeneralNameSpace();

            //// Запустить для инклюда
            BaseContextElement.CurrentSource = processingFileInfo.IncludeFilePath;
            var contextVisitor = new GlobalContextVisitor(contextElement);
            Analyzer.Analyze(processingFileInfo.IncludeFileTree, contextVisitor);

            // Запустить для основного
            BaseContextElement.CurrentSource = processingFileInfo.ProcessingFilePath;
            var contextVisitor2 = new GlobalContextVisitor(contextElement);
            Analyzer.Analyze(processingFileInfo.ProcessingFileTree, contextVisitor2);

            // Получить все что было в основном
            var classes = contextElement.Filter(x => x is ClassStructDeclaration
                                                    && x.Source.Equals(processingFileInfo.ProcessingFilePath))
                                        .Cast<ClassStructDeclaration>().ToList();

           
            foreach (var classItem in classes)
            {
                var classStructInfo = classItem.ClassStructInfo;
                CBOVertex cBOVertex;
                if (!Classes.TryGetValue(classStructInfo.GetFullName(), out cBOVertex))
                {
                    cBOVertex = Graph.CreateVertex();
                    cBOVertex.FullName = classStructInfo.GetFullName();
                    cBOVertex.Namespace = classStructInfo.GetNamespace();
                    Classes.Add(classStructInfo.GetFullName(), cBOVertex);
                }

                foreach (var field in classStructInfo.Fields.Where(x => x.Type is not null && x.Type.IsStandartType == false))
                {
                    ConnectType(field.Type, cBOVertex, classItem);
                    foreach (var item in field.Type.GetTemplateNamesList())
                    {
                        ConnectType(item, cBOVertex, classItem);
                    }
                }
            }

            var methods = contextElement.Filter(x => x is FunctionDeclaration
                                                   && x.Source.Equals(processingFileInfo.ProcessingFilePath))
                                       .Cast<FunctionDeclaration>()
                                       .Where(x => x.FunctionInfo.IsMethod == true)
                                       .ToList();

            foreach (var method in methods)
            {
                var classContext = method.GetTypeName(method.FunctionInfo.Name, method.FunctionInfo.NestedNames);

                if (classContext is not null && classContext is ClassStructDeclaration classStructDecl)
                {
                    
                    var classStructInfoForMethod = classStructDecl.ClassStructInfo;
                    CBOVertex cBOVertex;
                    if (!Classes.TryGetValue(classStructInfoForMethod.GetFullName(), out cBOVertex))
                    {
                        cBOVertex = Graph.CreateVertex();
                        cBOVertex.FullName = classStructInfoForMethod.GetFullName();
                        cBOVertex.Namespace = classStructInfoForMethod.GetNamespace();
                        Classes.Add(classStructInfoForMethod.GetFullName(), cBOVertex);
                    }
                    // параметры функции
                    foreach (var parameter in method.FunctionInfo.Parameters.Where(x => x.Type is not null && x.Type.IsStandartType == false))
                    {
                        ConnectType(parameter.Type, cBOVertex, classStructDecl);
                        foreach (var item in parameter.Type.GetTemplateNamesList())
                        {
                            ConnectType(item, cBOVertex, classStructDecl);
                        }
                    }
                    // Возвращаемое значение функции
                    if(method.FunctionInfo.ReturnType is not null)
                        ConnectType(method.FunctionInfo.ReturnType, cBOVertex, classStructDecl);


                    var allContext = method.Filter(x => true/*Получить все*/);
                    // Переменные в методе
                    var variables = allContext.SelectMany(x => x.VariableDeclaration)
                        .Where(x => x.Value.Type is not null
                        && x.Value.Type.IsStandartType == false).Select(x => x.Value) ;
                    
                    foreach (var variable in variables)
                    {
                        ConnectType(variable.Type, cBOVertex, classStructDecl);
                        foreach (var item in variable.Type.GetTemplateNamesList())
                        {
                            ConnectType(item, cBOVertex, classStructDecl);
                        }
                    }
                    // Используемые классы в методе
                    var usedTypes = allContext.SelectMany(x => x.UsedClasses).Where(x => x.IsStandartType == false);
                    foreach (var type in usedTypes)
                    {
                        ConnectType(type, cBOVertex, classStructDecl);
                        foreach (var item in type.GetTemplateNamesList())
                        {
                            ConnectType(item, cBOVertex, classStructDecl);
                        }
                    }
                }
                          
            }
            return true;
        } 
                
        public void Finalizer()
        {
            throw new NotImplementedException();
        }

        public string GenerateReport()
        {
            throw new NotImplementedException();
        }

    }
}
