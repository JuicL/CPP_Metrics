using CPP_Metrics.Types;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    public class XMLReport: IReportBuilder
    {
        public XMLReport(string outPath, List<MetricMessage> metricMessages)
        {
            OutPath = outPath;
            MetricMessages = metricMessages;
        }
        public string OutPath { get; }
        public List<MetricMessage> MetricMessages { get; }

        public ReportInfo ReportInfo => throw new NotImplementedException();

        public Config Config => throw new NotImplementedException();

        public string FileTag => throw new NotImplementedException();

        public override string GenerateBody()
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
                stringBuilder.AppendLine($"   <error id=\"{item.Id}\" severity=\"{WebUtility.HtmlEncode(item.Severity)}\" msg=\"{WebUtility.HtmlEncode(item.Message)}\" verbose=\"{WebUtility.HtmlEncode(item.Verbose)}\">");
                stringBuilder.AppendLine("    </error>");
            }
            stringBuilder.AppendLine("""
                    </errors>
                </results>
                """);
            var str = stringBuilder.ToString();

            return str;
        }
        public override string ReportBuild()
        {
            if (MetricMessages.Count == 0)
                return "";

            string fileIncludesPath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileIncludesPath = Path.Combine("/", "Report.xml");
            }
            else
            {
                fileIncludesPath = Path.Combine(OutPath, "Report.xml");
            }
            FileStream fileStream = new FileStream(fileIncludesPath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);

            var str = GenerateBody();
            streamWriter.Write(str);
            streamWriter.Close();
            fileStream.Close();

            return fileIncludesPath;
        }

      
    }
}
