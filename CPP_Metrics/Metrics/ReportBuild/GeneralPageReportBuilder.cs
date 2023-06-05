

using System.Text;

namespace CPP_Metrics.Metrics.ReportBuild
{
    public class GeneralPageReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; }

        public List<FileInfo> ProjectFiles = new();
        public List<MetricMessage> MetricMessages { get; set; } = new();
        public GeneralPageReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }

        public string FileTag { get; } = "index";

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"""
                        <div class="col-xl-6 col-lg-6 my-4">
                          <div class="bg-white rounded-lg p-5 shadow" style="height: 370px;">
                          <h2 class="h5 font-weight-bold text-center mb-4">Найдено проблем</h2>
          
                          <div class="progress mx-auto" data-value='100' >
                            <span class="progress-left" >
                              <span class="progress-bar border-danger"></span>
                            </span>
                            <span class="progress-right">
                              <span class="progress-bar border-danger"></span>
                            </span>
                            <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                              <div class="h2 font-weight-bold">{MetricMessages.Count}<sup class="small"></sup></div>
                            </div>
                          </div>
          
                          <div style="float:right"> 
                            <button id ="showHideContent" type="button" class="btn btn-primary mt-5" >Подробнее</button>
                          </div>
          
                        </div>
                      </div>

                      <div class="col-xl-6 col-lg-6 my-4">
                        <div class="bg-white rounded-lg p-5 shadow" style="height: 370px;">
                        <h2 class="h5 font-weight-bold text-center mb-4">Количество файлов для рефакторига</h2>
        
                        <div class="progress mx-auto" data-value='{0}'>
                          <span class="progress-left" >
                            <span class="progress-bar border-warning"></span>
                          </span>
                          <span class="progress-right">
                            <span class="progress-bar border-warning"></span>
                          </span>
                          <div class="progress-value w-100 h-100 rounded-circle d-flex align-items-center justify-content-center">
                            <div class="h2 font-weight-bold">{0}<sup class="small"></sup></div>
                          </div>
                        </div>
                        <div class="row text-center mt-4">
                          <div class="col-6 border-right">
                            <div class="h4 font-weight-bold mb-0">{0}</div>
                            <span class="small text-gray">Файлы содержащие проблемы</span>
                          </div>
                          <div class="col-6">
                            <div class="h4 font-weight-bold mb-0">{0}</div>
                            <span class="small text-gray">Общее количество файлов</span>
                          </div>
                        </div>
                      </div>
                    </div>
                    </div>
                """);

            //stringBuilder.Append($"<h4> Количество ошибок в проекте {MetricMessages.Count}</h4>");
            stringBuilder.Append("""
                <div id="content_h" style="display:none;">
                     <h2 class="h4 font-weight-bold mb-4">Список проблем проекта</h2>
                """);
            foreach (var item in MetricMessages)
            {
                stringBuilder.Append("<div class=\"alert alert-danger\">");
                stringBuilder.Append($"<strong>Error!</strong> {item.Message}</div>");
            }
            stringBuilder.Append("</div>");

            stringBuilder.Append("<h3 class=\"my-4\">Файлы проекта</h3>");
            stringBuilder.Append("<table class=\"table\">");
            stringBuilder.Append("<tbody>");
            foreach (var file in ProjectFiles)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{file.FullName}</th>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            
            return stringBuilder.ToString();
        }
    }
}
