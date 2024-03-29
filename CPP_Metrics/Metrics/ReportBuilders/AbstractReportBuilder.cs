﻿
using CPP_Metrics.Types;
using System.Collections.Concurrent;
using System.Text;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    public  class AbstractReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public ConcurrentDictionary<string, decimal> Result { get; set; }

        public string FileTag { get; } = "Abstract";

        public Config Config => throw new NotImplementedException();

        public List<MetricMessage> MetricMessages => throw new NotImplementedException();

        public AbstractReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }
        public override string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<h3 class=\"my-4\">Абстрактность категории</h3>");

            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<thead>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th style = \"width:80%\" scope = \"col\" > Категория </th>");
            stringBuilder.Append("<th style = \"width:20%\" scope = \"col\" > Значение </th>");

            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");

            stringBuilder.Append("<tbody>");
            foreach (var item in Result)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{item.Key}</th>");
                stringBuilder.Append($"<td>{item.Value}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        
    }
}
