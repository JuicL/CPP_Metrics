

using CPP_Metrics.Metrics;
using System.Text;

namespace CPP_Metrics
{
    public class XMLReport
    {
        public XMLReport(string outPath,List<MetricMessage> metricMessages)
        {
            OutPath = outPath;
            MetricMessages = metricMessages;
        }
        public string OutPath { get; }
        public List<MetricMessage> MetricMessages { get; }

        private string GenerateReport()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("""
                <?xml version="1.0" encoding="UTF-8"?>
                <results version="2">
                  <cppcheck version="1.52"/>
                  <errors>  
                """);
            foreach (var item in MetricMessages)
            {
                /*
                  <error id="Dit" severity="portability" msg="Dit  2warrning" verbose="Dit 2verbose">
                  </error>
                 */
                stringBuilder.AppendLine($"<error id=\"{item.Id}\" severity=\"{item.Severity}\" msg=\"{item.Message}\" verbose=\"{item.Verbose}\">");
                stringBuilder.AppendLine("</error>");
            }
            stringBuilder.AppendLine("""
                    </errors>
                </results>
                """);
            return stringBuilder.ToString();
        }
        public string ReportBuild()
        {
            if (MetricMessages.Count == 0)
                return "";

            var fileIncludesPath = Path.Combine(OutPath, "Report.html");

            FileStream fileStream = new FileStream(fileIncludesPath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            
            var str = GenerateReport();
            streamWriter.Write(str);
            streamWriter.Close();
            fileStream.Close();

            return fileIncludesPath;
        }

    }
}
