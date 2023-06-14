using Antlr4.Runtime.Misc;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Metrics;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;
using CPP_Metrics.DatabaseContext;

namespace CPP_Metrics.Metrics
{


    public class CBOMetric : IMetric
    {
        public CBOMetric()
        {

        }
        public CBOMetric(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }
        public IReportBuilder ReportBuilder { get ; set ; }
        public Dictionary<string, CBOVertex> Classes { get; set; } = new();
        public CBOGraph Graph { get; set; } = new();
        public List<MetricMessage> Messages { get; set; } = new ();
        public List<Pair<string, decimal>> Result { get; set; } = new();

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
                    cBOVertexForField.IsProjectClass = true;
                    cBOVertexForField.FullName = classStructInfoForField.GetFullName();
                    cBOVertexForField.Namespace = classStructInfoForField.GetNamespace();
                    Classes.Add(classStructInfoForField.GetFullName(), cBOVertexForField);
                }
                // Тип поля переменной является другой класс, связываем
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
            BaseContextElement.CurrentSource = processingFileInfo.ProcessingFilePath;
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
                    cBOVertex.IsProjectClass = true;
                    cBOVertex.FullName = classStructInfo.GetFullName();
                    cBOVertex.Namespace = classStructInfo.GetNamespace();
                    Classes.Add(classStructInfo.GetFullName(), cBOVertex);
                }
                // Обработка полей класса
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
                var nested = new List<CPPType>(method.FunctionInfo.NestedNames);
                var name = nested.Last();
                nested.RemoveAt(nested.Count-1);
                var classContext = method.GetTypeName(name.TypeName, nested);

                if (classContext is not null && classContext is ClassStructDeclaration classStructDecl)
                {
                    
                    var classStructInfoForMethod = classStructDecl.ClassStructInfo;
                    CBOVertex cBOVertex;
                    if (!Classes.TryGetValue(classStructInfoForMethod.GetFullName(), out cBOVertex))
                    {
                        cBOVertex = Graph.CreateVertex();
                        cBOVertex.IsProjectClass = true;
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
                    if(method.FunctionInfo.ReturnType is not null && method.FunctionInfo.ReturnType.IsStandartType == false)
                        ConnectType(method.FunctionInfo.ReturnType, cBOVertex, classStructDecl);


                    var allContext = method.Filter(x => true/*Получить все*/);
                    // Переменные в методе
                    var variables = allContext.SelectMany(x => x.VariableDeclaration)
                        .Where(x => x.Value.Type is not null
                        && x.Value.Type.IsStandartType == false).Select(x => x.Value) ;
                    
                    foreach (var variable in variables.Where(x => x.Type is not null && x.Type.IsStandartType == false))
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
        public Dictionary<string, int> Ca = new();
        public Dictionary<string, int> Ce = new();

        public void Finalizer()
        {
            //CBO
            foreach (var vertex in Graph.Verticies)
            {
                if (!vertex.IsProjectClass)
                    continue;

                // Исходящие зависимости
                var from = Graph.Edges.Where(x => x.From == vertex).Select(x => x.To).GroupBy(x => x.FullName).Select(x => x.First()).ToList();
                // Сколько классов ссылается на этот класс
                var to = Graph.Edges.Where(x => x.To == vertex).Select(x => x.From).GroupBy(x => x.FullName).Select(x => x.First()).ToList();
                Result.Add(new Pair<string, decimal>(vertex.FullName, from.Count + to.Count));

                if (vertex.Namespace is null) 
                    continue;

                //CA -- кол-во классов вне этой категории, которые зависят от классов внутри этой категории
                if (!Ca.TryGetValue(vertex.Namespace, out int ca_value))
                {
                    Ca.Add(vertex.Namespace, 0);
                }
                var out_category_ca = to.Where(x => vertex.Namespace != x.Namespace);
                Ca[vertex.Namespace] = out_category_ca.Count();

                //CE -- кол-во классов внутри категории которые зависят от классов вне этой категории
                if (!Ce.TryGetValue(vertex.Namespace, out int ce_value))
                {
                    Ce.Add(vertex.Namespace, 0);
                }
                var out_category_ce = from.Where(x => vertex.Namespace != x.Namespace);
                Ce[vertex.Namespace] = out_category_ce.Count();
                
            }
        }

        public string GenerateReport()
        {
            ((CBOReportBuilder)ReportBuilder).Result = Result;
            ReportBuilder.ReportBuild();
            return "";
        }

        public void Save(DbContextMetrics dbContext, Solution solution)
        {

        }
    }
}
