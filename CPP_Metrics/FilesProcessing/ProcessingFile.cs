
using CPP_Metrics.FilesPrepare;
using CPP_Metrics.Metrics;
using CPP_Metrics.Types;
using Facads;

namespace CPP_Metrics.FilesProcessing
{
    public class ProcessingFile
    {
        public List<string> SourceFilesPath { get; } = new List<string>();
        public Queue<FileInfo> ProcessingFilesQueue { get; } = new Queue<FileInfo>();

        public List<IMetric> Metrics = new List<IMetric>();
        public ProcessingFile(List<string> sourceFilesPath)
        {
            SourceFilesPath = sourceFilesPath;
        }

        private void RunMetrics(ProcessingFileInfo processingFileInfo)
        {
            foreach (var metric in Metrics)
            {
                metric.Handle(processingFileInfo);
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
            }
        }

        public void Run()
        {
            PrepareFiles prepareFiles = new PrepareFiles(SourceFilesPath);
            foreach (var item in prepareFiles.Files)
            {
                ProcessingFilesQueue.Enqueue(item.Value);
            }

            while (ProcessingFilesQueue.Count != 0)
            {
                var currentFile = ProcessingFilesQueue.Dequeue();

                ProcessingFileInfo processingFileInfo = new ProcessingFileInfo();
                processingFileInfo.FileInfo = currentFile;
                var preprocessingFile = prepareFiles.CreatePreprocessorFile(currentFile);
                var preprocessorFiles = prepareFiles.ReadPreprocessedFile(currentFile, preprocessingFile);


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
            // Если метрика требует пост операций, то вызываем финализацию подсчетов
            FinalizeMetrics();
            GenerateReport();
        }
        
    }
}
