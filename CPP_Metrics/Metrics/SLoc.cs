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

        public SLoc(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }

        

        public Dictionary<FileInfo, SLocInfo> SlocMetrics { get; set; } = new();

        
        

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            using (StreamReader sr = new StreamReader(processingFileInfo.FileInfo.FullName))
            {
                SLocInfo slocInfo = new();

                var str = sr.ReadLine();
                while (str is not null)
                {
                    slocInfo.Lines++;
                    if (str.Length == 0)
                    {
                        slocInfo.EmptyLines++;
                    }
                    else if (str.Contains("//"))
                    {
                        slocInfo.Commented++;
                    }
                    //else if(str.Contains("/*"))
                    //{
                    //    while (!str.Contains("*/"))
                    //    {
                    //        slocInfo.Commented = +1;
                    //        str = sr.ReadLine();
                    //    }
                    //    slocInfo.Commented = +1;
                        
                    //}
                    str = sr.ReadLine();
                }
                slocInfo.PercentСomment = Math.Round(((100m * slocInfo.Commented) / slocInfo.Lines), 2);
                slocInfo.PercentEmptyLines = Math.Round(((100m * slocInfo.EmptyLines) / slocInfo.Lines), 2);
                SlocMetrics.Add(processingFileInfo.FileInfo, slocInfo);
            }
            return true;
        }
        public void Finalizer()
        {
            // Общую стату посчитать
            FileInfo fileInfo = new FileInfo("|global|");// | недопустимый символ в названии файлов
            SLocInfo slocInfo = new();
            
            foreach (var item in SlocMetrics)
            {
                slocInfo.Lines += item.Value.Lines;
                slocInfo.EmptyLines += item.Value.EmptyLines;
                slocInfo.Commented += item.Value.Commented;

            }
            slocInfo.PercentСomment = Math.Round(((100m * slocInfo.Commented) / slocInfo.Lines), 2);
            slocInfo.PercentEmptyLines = Math.Round(((100m * slocInfo.EmptyLines) / slocInfo.Lines), 2);
            SlocMetrics.Add(fileInfo, slocInfo);

        }

        public string GenerateReport()
        {
            ((SlocReportBuilder)ReportBuilder).SlocMetrics = SlocMetrics;
            ReportBuilder.ReportBuild();
            return "";
        }

    }
}
