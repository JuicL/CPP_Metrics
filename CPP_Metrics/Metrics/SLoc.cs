using CPP_Metrics.DatabaseContext;
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
                    stringBuilder.AppendLine(line);
                }

                var str = stringBuilder.ToString();

                var m2 = regexCommnets.Matches(str);

                // 1 - /**/
                var miltiCommentLine = m2.Where(x => x.Groups[1].Captures.Count > 0).Select(x => x.Groups[1].Value);
                slocInfo.Commented += miltiCommentLine.Sum(x => x.Count(s => s == '\n')  + 1);

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

            foreach (var item in SlocMetrics)
            {
                //SLOCEmptyPercendId
                if (item.Value.PercentСomment > GlobalBoundaryValues.BoundaryValues.PercentCommented)
                {
                    Messages.Add(new MetricMessage()
                    {
                        Id = "SLOCCommentedPercendId",
                        Message = $"Percentage of commented code is greater than allowed. File {item.Key.Name} .Current {item.Value.PercentСomment} Threshold {GlobalBoundaryValues.BoundaryValues.PercentCommented} ",
                        MessageType = MessageType.Error
                    });
                }
                if (item.Value.PercentEmptyLines > GlobalBoundaryValues.BoundaryValues.PercentEmpty)
                {
                    Messages.Add(new MetricMessage()
                    {
                        Id = "SLOCEmptyPercendId",
                        Message = $"Percentage of empty code is greater than allowed. File {item.Key.Name}. Current {item.Value.PercentEmptyLines} Threshold {GlobalBoundaryValues.BoundaryValues.PercentEmpty} ",
                        MessageType = MessageType.Error
                    });
                }
            }
        }

        public string GenerateReport()
        {
            ((SlocReportBuilder)ReportBuilder).SlocMetrics = SlocMetrics;
            ReportBuilder.ReportBuild();
            return "";
        }

        

        public void Save(DbContextMetrics dbContext, Solution solution)
        {

            foreach (var sloc in SlocMetrics)
            {
                var value = sloc.Value;
                MetricValue line = new() { FileName = sloc.Key.FullName, ObjectName = sloc.Key.Name, SolutionID = solution.ID};
                line.Value = value.Lines;
                line.MetricDirectoryID = (int)dbContext.GetIdMetric("LOC");
                dbContext.MetricValues.Add(line);

                MetricValue comment = new() { FileName = sloc.Key.FullName, ObjectName = sloc.Key.Name, SolutionID = solution.ID };
                comment.Value = value.Commented;
                comment.MetricDirectoryID = (int)dbContext.GetIdMetric("LOCo");
                dbContext.MetricValues.Add(comment);

                MetricValue emptyLines = new() { FileName = sloc.Key.FullName, ObjectName = sloc.Key.Name, SolutionID = solution.ID };
                emptyLines.Value = value.EmptyLines;
                emptyLines.MetricDirectoryID = (int)dbContext.GetIdMetric("LOE");
                dbContext.MetricValues.Add(emptyLines);

                MetricValue percentСomment = new() { FileName = sloc.Key.FullName, ObjectName = sloc.Key.Name, SolutionID = solution.ID };
                percentСomment.Value = value.PercentСomment;
                percentСomment.MetricDirectoryID = (int)dbContext.GetIdMetric("PC");
                dbContext.MetricValues.Add(percentСomment);

                MetricValue percentEmptyLines = new() { FileName = sloc.Key.FullName, ObjectName = sloc.Key.Name, SolutionID = solution.ID };
                percentEmptyLines.Value = value.PercentEmptyLines;
                percentEmptyLines.MetricDirectoryID = (int)dbContext.GetIdMetric("PE");
                dbContext.MetricValues.Add(percentEmptyLines);
                dbContext.SaveChanges();

            }

        }
    }
}
