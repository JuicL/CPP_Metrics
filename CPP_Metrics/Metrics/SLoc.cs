using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Metrics
{
    public class SLocInfo
    {
        
    }

    internal class SLoc : IMetric
    {
        public IReportBuilder ReportBuilder { get ; set; }
        public SLoc()
        {

        }
        public Dictionary<string, SLocInfo> SlocMetrics { get; set; } = new();

        private void StrHandle(SLocInfo slocInfo, string str)
        {

        }

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            using (StreamReader sr = new StreamReader(processingFileInfo.FileInfo.FullName))
            {
                SLocInfo slocInfo = new();
                
                var str = sr.ReadLine();
                while (str is not null)
                {
                    StrHandle(slocInfo, str);
                    str = sr.ReadLine();
                }
                SlocMetrics.Add(processingFileInfo.FileInfo.FullName, slocInfo);
            }
            return true;
        }
        public void Finalizer()
        {
            // Общую стату посчитать
        }

        public string GenerateReport()
        {
            return "";
        }

    }
}
