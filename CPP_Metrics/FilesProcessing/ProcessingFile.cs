
using CPP_Metrics.FilesPrepare;
using CPP_Metrics.Metrics;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using Facads;

namespace CPP_Metrics.FilesProcessing
{
    public class ProcessingFile
    {
        public List<string> SourceFilesPath { get; } = new List<string>();
        public Dictionary<string, FileInfo> Files { get; set; } = new();
        public Queue<FileInfo> ProcessingFilesQueue { get; } = new Queue<FileInfo>();
        public ReportInfo ReportInfo { get; set; }
        public List<IMetric> Metrics { get; set; } = new List<IMetric>();
        protected string OutPath;
        public ProcessingFile(List<string> sourceFilesPath, string outPath)
        {
            SourceFilesPath = sourceFilesPath;
            OutPath = outPath;
        }

        private void RunMetrics(ProcessingFileInfo processingFileInfo)
        {
            var metricsThreads = new List<Thread>();

            foreach (var metric in Metrics)
            {
                var thread = new Thread(() =>
                {
                        metric.Handle(processingFileInfo);
                    try
                    {
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

            IReportBuilder generalReportBuilder = new GeneralPageReportBuilder(ReportInfo);
            List<MetricMessage> metricMessages = new();
            foreach (var metric in Metrics)
            {
                metric.GenerateReport();
                metricMessages.AddRange(metric.Messages);
            }
            ((GeneralPageReportBuilder)generalReportBuilder).MetricMessages.AddRange(metricMessages);
            ((GeneralPageReportBuilder)generalReportBuilder).ProjectFiles.AddRange(Files.Select(x => x.Value).ToList());

            generalReportBuilder.ReportBuild();
        }
        private void HandleFile(FileInfo fileInfo, PrepareFiles prepareFiles)
        {
            ProcessingFileInfo processingFileInfo = new ProcessingFileInfo();
            processingFileInfo.FileInfo = fileInfo;
            var preprocessingFile = prepareFiles.CreatePreprocessorFile(fileInfo);
            var preprocessorFiles = prepareFiles.ReadPreprocessedFile(fileInfo, preprocessingFile);


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
        public void Run()
        {
            PrepareFiles prepareFiles = new PrepareFiles(SourceFilesPath);
            Files = prepareFiles.Files;
            foreach (var item in prepareFiles.Files)
            {
                ProcessingFilesQueue.Enqueue(item.Value);
            }
            List<Thread> threads = new List<Thread>();  
            while (ProcessingFilesQueue.Count != 0)
            {
                var currentFile = ProcessingFilesQueue.Dequeue();
                Console.WriteLine("Processing file:" + currentFile.Name);
                HandleFile(currentFile, prepareFiles);
                //var thread = new Thread(() => HandleFile(currentFile, prepareFiles));
                //thread.Start();
                //threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            //Если метрика требует пост операций, то вызываем финализацию подсчетов
            FinalizeMetrics();
            GenerateReport();
        }
        
    }
}
