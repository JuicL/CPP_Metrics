

using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;

namespace CPP_Metrics.Metrics
{
    public class ClassAbstraction : IMetric
    {
        public IReportBuilder ReportBuilder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            // Создать контекст
            // Запустить для инклюда
            // Запустить для основного
            // Получить все что было в основном

            // Пройтись по классам...
            return true;
        }
        public void Finalizer()
        {
        }

        public string GenerateReport()
        {
            throw new NotImplementedException();
        }

    }
}
