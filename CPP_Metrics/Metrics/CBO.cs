

using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.Metrics
{
    public class CBOValue
    {
        public CPPType Type { get; set; }
        public int Value { get; set; }

    }
    public class CBOMetric : IMetric
    {
        public IReportBuilder ReportBuilder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<string, CBOValue> Metrics { get; set; }
        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            // У СПП ТИП поиском в глубину вывести в лист темлейты

            //Собрать все классы
            
            // В кадом классе взять пользовательские типы у полей, функций( параметры, выходное значение)

            // Собрать все функции

            // Найти для метода его класс
            //Поиском в глубину собрать все variable decl и usesType


            throw new NotImplementedException();
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
