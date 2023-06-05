using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{


    public class CombineMetric : ICombineMetric
    {
        //I = Ce / (Ce + Ca)

        // A
        //
        //      I(Instability)

        //D = A + I - 1 
        private CaCeMetric? CaCeMetric;
        private ClassAbstraction? ClassAbstraction;
        public Dictionary<string, decimal> Instability { get; set; } = new();
      
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
        public bool Handle(List<IMetric> metrics)
        {
            CaCeMetric = (CaCeMetric?)metrics.SingleOrDefault(x => x is CaCeMetric);
            ClassAbstraction = (ClassAbstraction?)metrics.SingleOrDefault(x => x is ClassAbstraction);
            if (CaCeMetric == null || ClassAbstraction == null) return false;

            foreach (var Ce in CaCeMetric.Ce)
            {
                CaCeMetric.Ca.TryGetValue(Ce.Key,out int ca);

            }






            return true;
        }

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            throw new NotImplementedException();
        }

    }
}
