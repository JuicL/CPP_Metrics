

using System.Text;

namespace CPP_Metrics.Metrics.ReportBuild
{
    public class GeneralPageReportBuilder : IReportBuilder
    {
        public ReportInfo ReportInfo { get; }

        public List<FileInfo> ProjectFiles = new();
        public List<MetricMessage> MetricMessages { get; set; } = new();
        public Config Config { get; set; }
        public GeneralPageReportBuilder(ReportInfo reportInfo)
        {
            ReportInfo = reportInfo;
        }

        public string FileTag { get; } = "index";

        public string GenerateBody()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"""
                <div class="row">
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
                          <div class="bg-white rounded-lg p-4 shadow" style="height: 370px;">
                            <h2 class="h4 font-weight-bold text-center mb-5">Общие сведения</h2>
            
                            <h2 class="h4 font-weight-bold text-center mb-5"></h2>

                            <p class="my-4" style="font-size: 22px;" >Количество файлов проекта: <strong>{ProjectFiles.Count}</strong> </p>
                            <p class="my-4" style="font-size: 22px;">Название проекта: {(Config.ProjectName is null ? "-" : Config.ProjectName)}</p>
                            <p class="my-4" style="font-size: 22px;">Пороговые значения: <strong>{(Config.BoundaryValues is null ? "default" : Config.BoundaryValues.Name) }</strong></p>
                            </div>
                        </div>

                 </div>
                """);

            //stringBuilder.Append($"<h4> Количество ошибок в проекте {MetricMessages.Count}</h4>");
            stringBuilder.AppendLine("""
                <div id="content_h" style="display:none;">
                     <h2 class="h4 font-weight-bold mb-4">Список проблем проекта</h2>
                """);
            foreach (var item in MetricMessages)
            {
                stringBuilder.AppendLine("<div class=\"alert alert-danger\">");
                stringBuilder.AppendLine($"<strong>Error!</strong> {item.Message}</div>");
            }
            stringBuilder.AppendLine("</div>");

            stringBuilder.AppendLine("<h3 class=\"my-4\">Файлы проекта</h3>");
            stringBuilder.AppendLine("<table class=\"table\">");
            stringBuilder.AppendLine("<tbody>");
            foreach (var file in ProjectFiles)
            {
                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine($"<td>{file.FullName}</th>");
                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");
            
            return stringBuilder.ToString();
        }
    }
}
