using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;
using CPP_Metrics.Types.DIT;

namespace CPP_Metrics.Metrics
{
    public class DIT : IMetric
    {
        public IReportBuilder ReportBuilder { get ; set ; }

        public DIT(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        // Граф
        DITGraph DITGraph { get; set; } = new DITGraph();
        public List<MetricMessage> Messages { get; set; } = new();

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            // Создать контекст
            //BaseContextElement.Clear();
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

            foreach (var classStruct in classes)
            {
                var fullNameClass = classStruct.ClassStructInfo.GetFullName();
                var addedVertex = DITGraph.Verticies.FirstOrDefault(x => x.Name.Equals(fullNameClass));
                if (addedVertex is null)
                {
                    addedVertex = DITGraph.CreateVertex();
                    addedVertex.Name = fullNameClass;
                }

                foreach (var basedClass in classStruct.ClassStructInfo.BaseClasses)
                {
                    var typeContext = classStruct.GetTypeName(basedClass.TypeName, basedClass.NestedNames);
                    if (typeContext is null)
                    {
                        throw new Exception($"Dont find type context {basedClass.TypeName}" );
                        
                        var secondChance = DITGraph.Verticies.SingleOrDefault(x => x.Name.EndsWith(basedClass.TypeName));
                        if(secondChance is null)
                        {
                            secondChance = DITGraph.CreateVertex();
                            secondChance.Name = basedClass.TypeName;
                        }
                        DITGraph.CreateEdge(secondChance, addedVertex);
                        continue;
                    }   


                    var baceClassFullName = ((ClassStructDeclaration)typeContext).ClassStructInfo.GetFullName();
                    var addedBacedClassVertex = DITGraph.Verticies.FirstOrDefault(x => x.Name.Equals(baceClassFullName)); // Warrning thread error
                    if(addedBacedClassVertex is null)
                    {
                        addedBacedClassVertex = DITGraph.CreateVertex();
                        addedBacedClassVertex.Name = baceClassFullName;
                    }

                    // Связать
                    lock ("DIT")
                    {
                        DITGraph.CreateEdge(addedBacedClassVertex, addedVertex);
                    }
                }

            }
            
            return true;
        }

        public void Finalizer()
        {

            var heads = DITGraph.Verticies.Where(x => !DITGraph.Edges.Any(e => e.To == x));
            Queue<DITVertex> queue = new();
            foreach (var head in heads)
            {
                queue.Enqueue(head);
            }
            while (queue.Count != 0)
            {
                var curVer = queue.Dequeue();

                foreach (var chlVer in DITGraph[curVer])
                {
                    chlVer.ParenCount = curVer.ParenCount + 1;
                    queue.Enqueue(chlVer);
                }
            }

            foreach (var item in DITGraph.Verticies)
            {
                if (item.ParenCount > GlobalBoundaryValues.BoundaryValues.DIT)
                {
                    Messages.Add(new MetricMessage()
                    {
                        MessageType = MessageType.Error,
                        Message = $"Depth of inheritance tree value is too high {item.Name}"
                    });
                }
            }
        }

        public string GenerateReport()
        {
            ((DITReportBuilder)ReportBuilder).DITGraph = DITGraph;
            ReportBuilder.ReportBuild();
            //Console.WriteLine("---DIT--");

            //foreach (var vertex in DITGraph.Verticies)
            //{
            //    Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
            //}
            return "";
        }
    }
}
