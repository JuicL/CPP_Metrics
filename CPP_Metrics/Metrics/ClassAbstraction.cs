

using CPP_Metrics.DatabaseContext;
using CPP_Metrics.Metrics.Contracts;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;
using CPP_Metrics.Visitors;
using System.Collections.Concurrent;
using System.Linq;

namespace CPP_Metrics.Metrics
{
    public class ClassAbstraction : IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public List<MetricMessage> Messages { get; set; } = new();

        public ClassAbstraction(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        private ConcurrentDictionary<string, List<ClassStructInfo>> NameSpaces = new ConcurrentDictionary<string, List<ClassStructInfo>>();

        public ConcurrentDictionary<string, decimal> Result = new ConcurrentDictionary<string, decimal>();
        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            // Создать контекст
            BaseContextElement.Clear();
            BaseContextElement.CurrentSource = processingFileInfo.ProcessingFilePath;
            BaseContextElement contextElement = BaseContextElement.GetGeneralNameSpace();

            //// Запустить для инклюда
            //contextElement.Source = processingFileInfo.IncludeFilePath;
            //var contextVisitor = new GlobalContextVisitor(contextElement);
            //Analyzer.Analyze(processingFileInfo.IncludeFileTree, contextVisitor);

            // Запустить для основного
            //contextElement.Source = processingFileInfo.ProcessingFilePath;
            var contextVisitor = new GlobalContextVisitor(contextElement);
            Analyzer.Analyze(processingFileInfo.ProcessingFileTree, contextVisitor);

            // Получить все что было в основном
            var namespaces = contextElement.Filter(x => x is NamespaceContext
                                                    && x.Source.Equals(processingFileInfo.ProcessingFilePath))
                                        .Cast<NamespaceContext>();
            foreach (var space in namespaces)
            {
                var classes = space.Children.Where(context => context is ClassStructDeclaration).Cast<ClassStructDeclaration>().ToList();
                var fullNameSpace = space.NameSpaceInfo.FullName();
                if(!NameSpaces.TryGetValue(fullNameSpace, out var value))
                {
                    NameSpaces.TryAdd(fullNameSpace, new List<ClassStructInfo>());
                }
                
                foreach (var item in classes)
                {
                    NameSpaces[fullNameSpace].Add(item.ClassStructInfo);
                }
            }
            return true;
        }

        public void Finalizer()
        {
            foreach (var item in NameSpaces)
            {
                decimal abstractClasses = item.Value.Count(x => x.IsAbstract);
                if (item.Value.Count == 0)
                {
                    Result.TryAdd(item.Key, 0);
                }
                else
                {
                    decimal abstractValue = abstractClasses / item.Value.Count;
                    Result.TryAdd(item.Key, abstractValue);
                }
            }
        }

        public string GenerateReport()
        {
            ((AbstractReportBuilder)ReportBuilder).Result = Result;
            ReportBuilder.ReportBuild();

            //Console.WriteLine("---Class Abstract--");
            //foreach (var item in Result)
            //{
            //    Console.WriteLine($"{item.Key} {item.Value}");
            //}
            return "";
        }

        public void Save(DbContextMetrics dbContext, Solution solution)
        {
            foreach (var item in Result)
            {
                var value = new MetricValue()
                {
                    SolutionID = solution.ID,
                    MetricDirectoryID = (int)dbContext.GetIdMetric("A"),
                    FileName = "",
                    ObjectName = item.Key,
                    Value = item.Value
                };
                dbContext.MetricValues.Add(value);
            }
            dbContext.SaveChanges();
        }
    }
}
