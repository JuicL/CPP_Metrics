﻿using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.Contracts;
using CPP_Metrics.Metrics.ReportBuilders;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Reflection;
using CPP_Metrics.Visitors;

namespace CPP_Metrics.Metrics
{
    public class CaCeMetric : IMetric
    {
        public CaCeMetric(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }
        public IReportBuilder ReportBuilder { get; set; }
        public Dictionary<string, CBOVertex> Classes { get; set; } = new();
        public CBOGraph Graph { get; set; } = new();
        public List<MetricMessage> Messages { get; set; } = new();
       
        public Dictionary<string, int> Ca { get; set; } = new();
        public Dictionary<string, int> Ce { get; set; } = new();
        private void ConnectType(CPPType? type, CBOVertex from, ClassStructDeclaration classItem)
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
                // наследование
                foreach (var item in classStructInfo.BaseClasses)
                {
                    ConnectType(item, cBOVertex, classItem);
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
                nested.RemoveAt(nested.Count - 1);
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
                    if (method.FunctionInfo.ReturnType is not null && method.FunctionInfo.ReturnType.IsStandartType == false)
                        ConnectType(method.FunctionInfo.ReturnType, cBOVertex, classStructDecl);


                    var allContext = method.Filter(x => true/*Получить все*/);
                    // Переменные в методе
                    var variables = allContext.SelectMany(x => x.VariableDeclaration)
                        .Where(x => x.Value.Type is not null
                        && x.Value.Type.IsStandartType == false).Select(x => x.Value);

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
        

        public void Finalizer()
        {
            Dictionary<string, List<CBOVertex>> ca = new();
            foreach (var vertex in Graph.Verticies)
            {
                if (!vertex.IsProjectClass)
                    continue;

                // Исходящие зависимости
                var from = Graph.Edges.Where(x => x.From == vertex).Select(x => x.To).GroupBy(x => x.FullName).Select(x => x.First()).ToList();
                // Сколько классов ссылается на этот класс
                var to = Graph.Edges.Where(x => x.To == vertex).Select(x => x.From).GroupBy(x => x.FullName).Select(x => x.First()).ToList();
                
                if (vertex.Namespace is null)
                    continue;

                //CA -- кол-во классов вне этой категории, которые зависят от классов внутри этой категории
                if (!ca.TryGetValue(vertex.Namespace, out var ca_value))
                {
                    ca.Add(vertex.Namespace, new());
                }
                ca[vertex.Namespace].AddRange(to);

                //CE -- кол-во классов внутри категории которые зависят от классов вне этой категории
                if (!Ce.TryGetValue(vertex.Namespace, out int ce_value))
                {
                    Ce.Add(vertex.Namespace, 0);
                }
                var out_category_ce = from.Where(x => vertex.Namespace != x.Namespace);
                if(out_category_ce.Count() != 0)
                    Ce[vertex.Namespace] += 1;

            }
            foreach (var item in ca)
            {
               var list = item.Value.Where(x=> x.Namespace != item.Key).GroupBy(x => x.FullName).Select(x => x.First()).ToList();
                Ca.Add(item.Key, list.Count);
            }

            
            foreach (var caItem in Ca)
            {
                if(caItem.Value > GlobalBoundaryValues.BoundaryValues.CA)
                {
                    Messages.Add(new MetricMessage()
                    {
                        Id = "CAId",
                        MessageType = MessageType.Error,
                        Message = $"Ca: Центростремительное сцепление превысило допустимое значение. Категория {caItem.Key}. Значение {caItem.Value}. Пороговое значение {GlobalBoundaryValues.BoundaryValues.CA}"
                    });
                }
            }
            foreach (var ceItem in Ce)
            {
                if (ceItem.Value > GlobalBoundaryValues.BoundaryValues.CE)
                {
                    Messages.Add(new MetricMessage()
                    {
                        Id = "CAId",
                        MessageType = MessageType.Error,
                        Message = $"Ce: Центробежное сцепление превысило допустимое значение. Категория {ceItem.Key}. Значение {ceItem.Value}. Пороговое значение {GlobalBoundaryValues.BoundaryValues.CE}"
                    });
                }
            }
        }

        public string GenerateReport()
        {
            ((CaCeReportBuilder)ReportBuilder).Ca = Ca;
            ((CaCeReportBuilder)ReportBuilder).Ce = Ce;

            ReportBuilder.ReportBuild();
            return "";
        }

        public void Save(DbContextMetrics dbContext, Solution solution)
        {
            foreach (var ca in Ca)
            {
                var value = new MetricValue()
                {
                    SolutionID = solution.ID,
                    MetricDirectoryID = (int)dbContext.GetIdMetric("CA"),
                    FileName = "",
                    ObjectName = ca.Key,
                    Value = ca.Value
                };
                dbContext.MetricValues.Add(value);
            }
            foreach (var ce in Ce)
            {
                var value = new MetricValue()
                {
                    SolutionID = solution.ID,
                    MetricDirectoryID = (int)dbContext.GetIdMetric("CE"),
                    FileName = "",
                    ObjectName = ce.Key,
                    Value = ce.Value
                };
                dbContext.MetricValues.Add(value);
            }
            dbContext.SaveChanges();
        }
    }
}
