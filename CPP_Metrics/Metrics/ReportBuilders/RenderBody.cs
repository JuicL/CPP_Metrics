namespace CPP_Metrics.Metrics.ReportBuilders
{
    public class RenderBody
    {
        public static string Header { get 
            { 
                return
                    """
                    <!DOCTYPE html>
                    
                    <html lang="en">
                    <head>
                        <meta charset="UTF-8">
                        <meta http-equiv="X-UA-Compatible" content="IE=edge">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.3.1/dist/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
                        <link rel="stylesheet" href="http://d.zaix.ru/AniW.css" />
                        <title>Document</title>
                    </head>
                    <header>
                        <!-- Sidebar -->
                        <nav
                             id="sidebarMenu"
                             class="collapse d-lg-block sidebar collapse bg-white"
                             >
                          <div class="position-sticky">
                            <div class="list-group list-group-flush mx-3 mt-4">
                              <a
                                 href="index.html"
                                 class="list-group-item list-group-item-action py-2 ripple"
                                 aria-current="true"
                                 >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                  ><span>Главная</span>
                              </a>
                              <a
                                 href="Cyclomatic.html"
                                 class="list-group-item list-group-item-action py-2 ripple"
                                 >
                                <i class="fas fa-chart-area fa-fw me-3"></i
                                  ><span>Цикломатическая</span>
                              </a>
                              <a
                                 href="Sloc.html"
                                 class="list-group-item list-group-item-action py-2 ripple"
                                 aria-current="true"
                                 >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                  ><span>SLOC</span>
                              </a>
                              <a
                                 href="DIT.html"
                                 class="list-group-item list-group-item-action py-2 ripple"
                                 aria-current="true"
                                 >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                  ><span>Наследование</span>
                              </a>
                              <a
                                 href="Abstract.html"
                                 class="list-group-item list-group-item-action py-2 ripple"
                                 aria-current="true"
                                 >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                  ><span>Абстрактность</span>
                              </a>
                              <a
                                 href="CBO.html"
                                 class="list-group-item list-group-item-action py-2 ripple"
                                 aria-current="true"
                                 >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                  ><span>CBO</span>
                              </a>
                                <a
                                    href="CaCe.html"
                                    class="list-group-item list-group-item-action py-2 ripple"
                                    aria-current="true"
                                    >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                    ><span>CaCe</span>
                               </a>
                               </a>
                                <a
                                    href="Instability.html"
                                    class="list-group-item list-group-item-action py-2 ripple"
                                    aria-current="true"
                                    >
                                <i class="fas fa-tachometer-alt fa-fw me-3"></i
                                    ><span>Главная последовательность</span>
                               </a>
                            </div>
                          </div>
                        </nav>
                      <!-- Navbar -->
                      <nav id="main-navbar" class="navbar navbar-expand-lg navbar-light  fixed-top">
                        <!-- Container wrapper -->
                        <div class="container-fluid">
                          <!-- Toggle button -->
                          <button class="navbar-toggler" type="button" data-mdb-toggle="collapse" data-mdb-target="#sidebarMenu"
                            aria-controls="sidebarMenu" aria-expanded="false" aria-label="Toggle navigation">
                            <i class="fas fa-bars"></i>
                          </button>

                          <!-- Brand -->
                          <a class="navbar-brand" href="#">
                            <img src="http://d.zaix.ru/AnjP.png"  height="50px"/>
                          </a>
                          </ul>
                        </div>
                        <!-- Container wrapper -->
                      </nav>
                      <!-- Navbar -->
                      </header>
                    <body>
                      <main style="margin-top: 58px">
                        <div class="container pt-4">
                    """; 
            } }
        public static string Footer
        {
            get
            {
                return
                    """
                    </div>
                    </main>
                    

                    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
                    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.14.7/dist/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
                    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.3.1/dist/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
                    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js"></script>
                    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
                    
                    <script>
                        

                        $(document).ready(function(){
                            $("#showHideContent").click(function () {
                                if ($("#content_h").is(":hidden")) {
                                    $("#content_h").show("slow");
                                } else {
                                    $("#content_h").hide("slow");
                                }
                                return false;
                            });
                        });

                        $(function() {
                          $(".progress").each(function() {

                            var value = $(this).attr('data-value');
                            var left = $(this).find('.progress-left .progress-bar');
                            var right = $(this).find('.progress-right .progress-bar');

                            if (value > 0) {
                              if (value <= 50) {
                                right.css('transform', 'rotate(' + percentageToDegrees(value) + 'deg)')
                              } else {
                                right.css('transform', 'rotate(180deg)')
                                left.css('transform', 'rotate(' + percentageToDegrees(value - 50) + 'deg)')
                              }
                            }
                          })

                          function percentageToDegrees(percentage) {

                            return percentage / 100 * 360

                          }

                        });
                        </script>

                    </body>
                    </html>
                    """;
            }
        }
    };
}
