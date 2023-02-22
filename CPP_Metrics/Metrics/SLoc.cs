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
        public int Lines { get; set; }
        public int Commented { get; set; }
        public int EmptyLines { get; set; }
        public decimal PercentСomment { get; set; }
        public decimal PercentEmptyLines { get; set; }
    }

    public class SLoc : IMetric
    {
        public IReportBuilder ReportBuilder { get; set; }
        public SLoc()
        {
        }

        public Dictionary<string, SLocInfo> SlocMetrics { get; set; } = new();

        private void StrHandle(SLocInfo slocInfo, string str)
        {
            if (str.Length == 0)
            {
                slocInfo.EmptyLines = +1;
            }
            else if (str.Contains("//"))
            {
                slocInfo.Commented = +1;
            }

        }
        /*
            
        */

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            using (StreamReader sr = new StreamReader(processingFileInfo.FileInfo.FullName))
            {
                SLocInfo slocInfo = new();

                var str = sr.ReadLine();
                while (str is not null)
                {
                    if (str.Length == 0)
                    {
                        slocInfo.EmptyLines = +1;
                    }
                    else if (str.Contains("//"))
                    {
                        slocInfo.Commented = +1;
                    }
                    else if(str.Contains("/*"))
                    {
                        while (!str.Contains("*/"))
                        {
                            slocInfo.Commented = +1;
                            str = sr.ReadLine();
                        }
                        slocInfo.Commented = +1;

                    }
                    str = sr.ReadLine();
                }

                SlocMetrics.Add(processingFileInfo.FileInfo.FullName, slocInfo);
            }
            return true;
        }
        public void Finalizer()
        {
            SLocInfo slocInfo = new();
            // Общую стату посчитать
            
            SlocMetrics.Add("|global|", slocInfo);// | недопустимый символ в названии файлов

        }

        public string GenerateReport()
        {
            return "";
        }

    }
}
