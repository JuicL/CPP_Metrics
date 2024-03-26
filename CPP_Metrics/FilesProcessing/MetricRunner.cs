using Antlr4.Runtime.Misc;
using CPP_Metrics.DatabaseContext;
using CPP_Metrics.FilesPrepare;
using CPP_Metrics.Metrics.Contracts;
using CPP_Metrics.Metrics.ReportBuilders;
using CPP_Metrics.Types;
using Facads;

namespace CPP_Metrics.FilesProcessing
{
    public class MetricRunner
    {
        private Config Config { get; set; }
        public List<string> SourceFilesPath { get; } = new List<string>();
        public Dictionary<string, FileInfo> Files { get; set; } = new();
        public Queue<FileInfo> ProcessingFilesQueue { get; } = new Queue<FileInfo>();
        public ReportInfo ReportInfo { get; set; }
        public List<IMetric> Metrics { get; set; } = new();
        public List<ICombineMetric> CombineMetrics { get; set; } = new();
        public List<IReportBuilder> ReportBuilders { get; set; }

        public List<MetricMessage> MetricMessages = new();
        
        public MetricRunner(List<string> sourceFilesPath, Config config, ReportInfo reportInfo)
        {
            SourceFilesPath = sourceFilesPath;
            Config = config;
            ReportInfo = reportInfo;
        }
        
        private void RunMetrics(ProcessingFileInfo processingFileInfo)
        {
            var metricsThreads = new List<Thread>();

            foreach (var metric in Metrics)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        metric.Handle(processingFileInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                thread.Start();
                metricsThreads.Add(thread);
            }

            foreach(var thread in metricsThreads)
            {
                thread.Join();
            }
            
        }

        private void FinalizeMetrics()
        {
            foreach (var metric in Metrics)
            {
                metric.Finalizer();
            }
            
        }

        // Генерация отчетов
        public void GenerateReport()
        {

            foreach (var metric in Metrics)
            {
                metric.GenerateReport();
                MetricMessages.AddRange(metric.Messages);
            }

            foreach (var reportBuilder in ReportBuilders)
            {
                reportBuilder.ReportBuild();
            }
           
        }

        private void HandleFile(FileInfo fileInfo, PrepareFiles prepareFiles)
        {
            ProcessingFileInfo processingFileInfo = new ProcessingFileInfo();
            processingFileInfo.FileInfo = fileInfo;
            var preprocessingFile = prepareFiles.CreatePreprocessorFile(fileInfo);
            Pair<string, string>? preprocessorFiles = null;
            try
            {
                preprocessorFiles = prepareFiles.ReadPreprocessedFile(fileInfo, preprocessingFile);
            }
            catch (Exception)
            {
                Console.WriteLine($"Preprocesser fail: {fileInfo.FullName}");
                return;
            }

            if (preprocessorFiles is null) return;
            
            //using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            processingFileInfo.IncludeFilePath = preprocessorFiles.a;
            using (StreamReader sr = new StreamReader(processingFileInfo.IncludeFilePath))
            {
                processingFileInfo.IncludeFile = sr.ReadToEnd();
                Facad facad = new Facad(processingFileInfo.IncludeFile);
                processingFileInfo.IncludeFileTree = facad.GetTree();
            }

            processingFileInfo.ProcessingFilePath = preprocessorFiles.b;
            using (StreamReader sr = new StreamReader(processingFileInfo.ProcessingFilePath))
            {
                processingFileInfo.ProcessingFile = sr.ReadToEnd();
                Facad facad = new Facad(processingFileInfo.ProcessingFile);
                processingFileInfo.ProcessingFileTree = facad.GetTree();
            }
            // Запускаем сбор метрик для файла
            RunMetrics(processingFileInfo);
        }
        private void RunCombineMetrics()
        {
            var metricsThreads = new List<Thread>();

            foreach (var combineMetric in CombineMetrics)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        combineMetric.Handle(Metrics);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                thread.Start();
                metricsThreads.Add(thread);
            }

            foreach (var thread in metricsThreads)
            {
                thread.Join();
            }

            foreach (var combineMetric in CombineMetrics)
            {
                combineMetric.Finalizer();
            }

            foreach (var combineMetric in CombineMetrics)
            {
                combineMetric.GenerateReport();
                MetricMessages.AddRange(combineMetric.Messages);
            }
        }
        public void Run()
        {
            PrepareFiles prepareFiles = new PrepareFiles(SourceFilesPath, Config);
            Files = prepareFiles.Files;

            var boundaryFile = prepareFiles.ConfigFiles.Where(x => x.Value.Extension == ".cppconfig")?.SingleOrDefault().Value;
            if (boundaryFile is not null)
            {
                Config.BoundaryValues = boundaryFile;
                var boundaryValue = BoundaryValuesReader.ReadConfigFile(boundaryFile.FullName);
                if(boundaryValue is not null)
                {
                    GlobalBoundaryValues.BoundaryValues = boundaryValue;
                }
            }

            foreach (var item in prepareFiles.Files)
            {
                ProcessingFilesQueue.Enqueue(item.Value);
            }
            List<Thread> threads = new List<Thread>();  
            while (ProcessingFilesQueue.Count != 0)
            {
                var currentFile = ProcessingFilesQueue.Dequeue();
                Console.WriteLine("Processing file:" + currentFile.Name);
                //HandleFile(currentFile, prepareFiles);
                var thread = new Thread(() => HandleFile(currentFile, prepareFiles));
                thread.Start();
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            //Если метрика требует пост операций, то вызываем финализацию подсчетов
            FinalizeMetrics();
            GenerateReport();
            RunCombineMetrics();
            Save();
        }
        private void Save()
        {
            if (Config.ProjectName is null || Config.ProjectName.Length == 0)
                return;

            using (var db = new DbContextMetrics())
            {
                var project = db.Projects.FirstOrDefault(x => x.Name == Config.ProjectName);
                if (project is null)
                {
                    project = new Project() { Name = Config.ProjectName, Date = DateTime.UtcNow };
                    db.Projects.Add(project);
                    db.SaveChanges();
                }

                var solution = new Solution() { ProjectID = project.ID, Date = DateTime.UtcNow };
                db.Solutions.Add(solution);
                db.SaveChanges();
                
                foreach (var item in Metrics)
                {
                    item.Save(db, solution);
                }

                foreach (var item in CombineMetrics)
                {
                    item.Save(db, solution);
                } 
            }
        }
    }
}
