using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Tool;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;
using CPP_Metrics.Types.DIT;

namespace CPP_Metrics.Metrics
{
    public class DIT : IMetric
    {
        public IReportBuilder ReportBuilder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // Граф
        DITGraph DITGraph { get; set; } = new DITGraph(); 
        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            // Создать контекст
            BaseContextElement.Clear();
            BaseContextElement contextElement = BaseContextElement.GetGeneralNameSpace();

            // Запустить для инклюда
            BaseContextElement.CurrentSource = processingFileInfo.IncludeFilePath;
            var contextVisitor = new GlobalContextVisitor(contextElement);
            Analyzer.Analyze(processingFileInfo.IncludeFileTree, contextVisitor);

            // Запустить для основного
            BaseContextElement.CurrentSource = processingFileInfo.ProcessingFilePath;
            contextVisitor = new GlobalContextVisitor(contextElement);
            Analyzer.Analyze(processingFileInfo.ProcessingFileTree, contextVisitor);

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
                        throw new Exception("Dont find type context");
                    
                    var baceClassFullName = ((ClassStructDeclaration)typeContext).ClassStructInfo.GetFullName();
                    var addedBacedClassVertex = DITGraph.Verticies.FirstOrDefault(x => x.Name.Equals(baceClassFullName));
                    if(addedBacedClassVertex is null)
                    {
                        addedBacedClassVertex = DITGraph.CreateVertex();
                        addedBacedClassVertex.Name = baceClassFullName;
                    }

                    // Связать
                    DITGraph.CreateEdge(addedBacedClassVertex, addedVertex);
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
        }

        public string GenerateReport()
        {
            Console.WriteLine("---DIT--");

            foreach (var vertex in DITGraph.Verticies)
            {
                Console.WriteLine($"{vertex.Name} {vertex.ParenCount}");
            }
            return "";
        }
    }
}
