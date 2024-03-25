using System.Collections.Concurrent;
using System.Text;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    internal class SlocReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; set; }
        public string FileTag { get; set; } = "Sloc";
        public ConcurrentDictionary<FileInfo, SLocInfo> SlocMetrics { get; set; } = new();

        public SlocReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }
        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<h3 class=\"my-4\">Количественные метрики SLOC</h3>");
            stringBuilder.AppendLine("<h4 class=\"my-4\">Общее</h4>");
            
            SLocInfo sLocInfo = SlocMetrics.Single(x => x.Key.Name == "|global|").Value;

            stringBuilder.AppendLine($"""
                <div class="row">
                      <div class="col-xl-3  my-4">
                        <div class="bg-white rounded-lg p-5 shadow">
                          <h2 class="h5 font-weight-bold text-center mb-4">Количество строк</h2>
  
                          <!-- Progress bar 1 -->
                          <div class="progress mx-auto" data-value='100'>
                            <span class="progress-left" >
                                  <span class="progress-bar border-success"></span>
                            </span>
                            <span class="progress-right">
                                          <span class="progress-bar border-success"></span>
                            </span>
                            <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                              <div class="h2 font-weight-bold">{sLocInfo.Lines}</div>
                            </div>
                          </div>
                        </div>
                      </div>
                      <div class="col-xl-3  my-4">
                        <div class="bg-white rounded-lg p-5 shadow">
                          <h2 class="h5 font-weight-bold text-center mb-4">Количество комментариев</h2>
  
                          <!-- Progress bar 1 -->
                          <div class="progress mx-auto" data-value='100'>
                            <span class="progress-left" >
                                  <span class="progress-bar border-success"></span>
                            </span>
                            <span class="progress-right">
                                          <span class="progress-bar border-success"></span>
                            </span>
                            <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                              <div class="h2 font-weight-bold">{sLocInfo.Commented}<sup class="small"></sup></div>
                            </div>
                          </div>
                        </div>
                      </div>
                      <div class="col-xl-3 col-lg-6 my-4">
                        <div class="bg-white rounded-lg p-5 shadow">
                          <h2 class="h5 font-weight-bold text-center mb-4">Количество пустых строк</h2>
  
                          <!-- Progress bar 1 -->
                          <div class="progress mx-auto" data-value='100'>
                            <span class="progress-left" >
                                  <span class="progress-bar border-success"></span>
                            </span>
                            <span class="progress-right">
                                          <span class="progress-bar border-success"></span>
                            </span>
                            <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                              <div class="h2 font-weight-bold">{sLocInfo.EmptyLines}<sup class="small"></sup></div>
                            </div>
                          </div>
                        </div>
                      </div>
                  </div>
                """);
            stringBuilder.Append($"""
                <div class="row">
                  <div class="col-xl-3  my-4">
                    <div class="bg-white rounded-lg p-5 shadow">
                      <h2 class="h5 font-weight-bold text-center mb-4">Процент комментариев</h2>
                      <div class="progress mx-auto" data-value='{(int)sLocInfo.PercentСomment}'>
                        <span class="progress-left" ><span class="progress-bar border-warning"></span></span>
                        <span class="progress-right"><span class="progress-bar border-warning"></span></span>
                        <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                          <div class="h2 font-weight-bold">{sLocInfo.PercentСomment}<sup class="small">%</sup></div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-xl-3  my-4">
                    <div class="bg-white rounded-lg p-5 shadow">
                      <h2 class="h5 font-weight-bold text-center mb-4">Процент пустых строк</h2>
                      <div class="progress mx-auto" data-value='{(int)sLocInfo.PercentEmptyLines}'>
                        <span class="progress-left" ><span class="progress-bar border-warning"></span></span>
                        <span class="progress-right"><span class="progress-bar border-warning"></span></span>
                        <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                          <div class="h2 font-weight-bold">{sLocInfo.PercentEmptyLines}<sup class="small">%</sup></div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                """);


        stringBuilder.Append("<h3 class=\"my-4\">Файлы проекта</h3>");

                stringBuilder.Append("<table class=\"table\">");
                stringBuilder.Append("<thead>");
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<th style = \"width:50%\" scope = \"col\" > Файл </th>");

                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Lines </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Comments </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > Empty </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > %Comment </th>");
                stringBuilder.Append("<th style = \"width:10%\" scope = \"col\" > %Empty </th>");


                stringBuilder.Append("</tr>");
                stringBuilder.Append("</thead>");

                stringBuilder.Append("<tbody>");
                foreach (var item in SlocMetrics.Where(x => x.Key.Name != "|global|"))
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append($"<td>{item.Key.Name}</th>");
                    stringBuilder.Append($"<td>{item.Value.Lines}</th>");
                    stringBuilder.Append($"<td>{item.Value.Commented}</th>");
                    stringBuilder.Append($"<td>{item.Value.EmptyLines}</th>");
                    stringBuilder.Append($"<td>{item.Value.PercentСomment}</th>");
                    stringBuilder.Append($"<td>{item.Value.PercentEmptyLines}</th>");
                    stringBuilder.Append("</tr>");
                }

                stringBuilder.Append("</tbody>");
                stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        
    }
}
