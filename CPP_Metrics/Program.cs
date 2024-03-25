using CPP_Metrics.DatabaseContext;
using CPP_Metrics.FilesProcessing;
using CPP_Metrics.Metrics;
using CPP_Metrics.Metrics.ReportBuilders;
using CPP_Metrics.Types;
using System.Text;

class CppMetrics
{
    static public void HandleArguments(Config config, string[] args)
    {
        for (int i = 0; i < args.Length;)
        {
            switch (args[i])
            {
                case "-cp":
                    i++;
                    string? newprjname = null;
                    if (i < args.Length && !args[i].StartsWith('-'))
                        newprjname = args[i];
                    Repository.CreateProjects(newprjname);
                    i++;
                    break;
                case "-gp":
                    i++;
                    string? prjname = null;
                    if (i < args.Length && !args[i].StartsWith('-'))
                        prjname = args[i];
                    Repository.GetProjects(prjname);
                    i++;
                    break;
                case "-gs":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected projectId");
                    int prjId;
                    try
                    {
                        prjId = int.Parse(args[i]);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Expected projectId");
                    }
                    Repository.GetSolution(prjId);
                    i++;
                    break;
                case "-gm":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected project Id");
                    int solId;
                    try
                    {
                        solId = int.Parse(args[i]);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Expected solution Id");
                    }
                    Repository.GetMetrics(solId);
                    i++;
                    break;
                case "-i":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected path to additional processor files");
                    config.CompilerAddFiles.Add(args[i]);
                    i++;
                    break;
                case "-f":
                    i++;
                    do
                    {
                        if (args[i].StartsWith('-') || i >= args.Length)
                            throw new Exception("Expected path to project files");
                        config.ProjectFiles.Add(args[i]);
                        i++;
                    } while (i < args.Length && !args[i].StartsWith('-'));

                    break;
                case "-o":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-') )
                        throw new Exception("Expected path to out report files");
                    config.OutReportPath = args[i];
                    i++;
                    break;
                case "-p":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected project name");
                    config.ProjectName = args[i];
                    i++;
                    break;
                case "-up":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected project name");
                    var name = args[i];
                    i++;
                    if (args[i].StartsWith('-') || i >= args.Length)
                        throw new Exception("Expected new project name");
                    var newName = args[i];
                    Repository.UpdateProject(name,newName);
                    i++;
                    break;
                case "-dp":
                    // Delete
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected project name");
                    var deleteName = args[i];
                    Repository.DeleteProject(deleteName);
                    i++;
                    break;
                case "-cfg":
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected path to cfg file");
                    var cfgFilePath = args[i];
                    var path = Path.Combine(cfgFilePath, ".cppconfig");
                    try
                    {
                        BoundaryValuesWriter.CreateConfigFile(path);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine( ex.Message);
                    }
                    i++;
                    break;
                default:
                    throw new Exception($"Invalid parameter {args[i]}");

            }
        }
    }

    public static List<MetricMessage> MetricMessages= new List<MetricMessage>();
    private static void RunMetrics(List<string> sourcePaths, Config config)
    {
        if (sourcePaths.Count == 0)
            throw new Exception("Empty sourse path list");
        var reportInfo = ReportPreparer.Prepare(config);

        var metricRunner = new MetricRunner(sourcePaths, config, reportInfo);
        
        var SLocReport = new SlocReportBuilder(reportInfo);
        metricRunner.Metrics.Add(new SLocMetric(SLocReport));

        var cyclomaticReport = new CyclomaticReportBuilder(reportInfo);
        metricRunner.Metrics.Add(new CylomaticComplexityMetric(cyclomaticReport));

        var classAbstractionBuilder = new AbstractReportBuilder(reportInfo);
        metricRunner.Metrics.Add(new ClassAbstractionMetric(classAbstractionBuilder));

        var DITReport = new DITReportBuilder(reportInfo);
        metricRunner.Metrics.Add(new DITMetric(DITReport));

        var CBOReport = new CBOReportBuilder(reportInfo);
        metricRunner.Metrics.Add(new CBOMetric(CBOReport));
        
        var CaCeReport = new CaCeReportBuilder(reportInfo);
        metricRunner.Metrics.Add(new CaCeMetric(CaCeReport));
        
        // Комбинированные метрики
        var instabilityReport = new InstabilityReportBuilder(reportInfo);
        metricRunner.CombineMetrics.Add(new InstabilityMetric(instabilityReport));
        
        metricRunner.Run();
        MetricMessages.AddRange(metricRunner.MetricMessages);
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Cpp metrics running");
        
        foreach (var item in args)
        {
            Console.WriteLine($"arg: {item}");
        }

        Config config = new Config();
        if (args.Length != 0)
        {
            HandleArguments(config, args);
            RunMetrics(config.ProjectFiles,config);
            
            Console.WriteLine("Cpp metrics finished");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\n======================================================================================");
            stringBuilder.AppendLine("                                     PROBLEMS                                           ");
            stringBuilder.AppendLine("======================================================================================");

            foreach (var item in MetricMessages.Where(x=> x.MessageType == MessageType.Error)) {
                stringBuilder.AppendLine(item.Message);
            }

            if (stringBuilder.Length > 0)
            {
                var str = stringBuilder.ToString();
                throw new Exception(str);
            }

            return;
        }
        throw new Exception("Empty arguments");
        
    }
}
