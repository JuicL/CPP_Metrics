using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public SLoc(IReportBuilder reportBuilder)
        {
            ReportBuilder = reportBuilder;
        }
        public IReportBuilder ReportBuilder { get; set; }
        public ConcurrentDictionary<FileInfo, SLocInfo> SlocMetrics { get; set; } = new();
        public List<MetricMessage> Messages { get; set; } = new();

        public bool Handle(ProcessingFileInfo processingFileInfo)
        {
            var regexCommnets = new Regex(@"(\/\*(.*\n)*\*\/)|(\/\/.*\n(\s*\r\n)*)|("".*"")|(\n\s*\r\n)");

            using (StreamReader sr = new StreamReader(processingFileInfo.FileInfo.FullName))
            {
                SLocInfo slocInfo = new();
                string? line;
                StringBuilder stringBuilder = new();
                while ((line = sr.ReadLine()) != null)
                {
                    slocInfo.Lines++;
                    stringBuilder.Append(line);
                }

                var str = stringBuilder.ToString();

                var m2 = regexCommnets.Matches(str);

                // 1 - /**/
                var miltiCommentLine = m2.Where(x => x.Groups[1].Captures.Count > 0).Select(x => x.Groups[1].Value);
                slocInfo.Commented += miltiCommentLine.Sum(x => x.Count(s => s == '\n'));

                // 4 - empty line after //
                var EmptyLine = m2.Where(x => x.Groups[4].Captures.Count > 0).Select(x => x.Groups[4].Value);
                slocInfo.EmptyLines += EmptyLine.Sum(x => x.Count(s => s == '\n'));

                // 3 - //
                var CommentLine = m2.Where(x => x.Groups[3].Captures.Count > 0).Select(x => x.Groups[3].Value);
                slocInfo.Commented += CommentLine.Count();

                // 6 - empty line
                var EmptyLine2 = m2.Where(x => x.Groups[6].Captures.Count > 0).Select(x => x.Groups[6].Value);
                slocInfo.EmptyLines += EmptyLine2.Sum(x => x.Count(s => s == '\n') - 1);

                slocInfo.PercentСomment = Math.Round(((100m * slocInfo.Commented) / slocInfo.Lines), 2);
                slocInfo.PercentEmptyLines = Math.Round(((100m * slocInfo.EmptyLines) / slocInfo.Lines), 2);
                SlocMetrics.TryAdd(processingFileInfo.FileInfo, slocInfo);
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
            SlocMetrics.TryAdd(fileInfo, slocInfo);

        }

        public string GenerateReport()
        {
            ((SlocReportBuilder)ReportBuilder).SlocMetrics = SlocMetrics;
            ReportBuilder.ReportBuild();
            return "";
        }

    }
}
