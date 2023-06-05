using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{


    public class CombineMetric : IMetric
    {
        public CombineMetric(List<IMetric> metrics)
        {
            
        }

        public IReportBuilder ReportBuilder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<MetricMessage> Messages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Finalizer()
        {
            throw new NotImplementedException();
        }

        public string GenerateReport()
        {
            throw new NotImplementedException();
        }

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
