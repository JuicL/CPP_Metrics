
using ConsoleTables;
using CPP_Metrics;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.DatabaseContext;
using CPP_Metrics.FilesPrepare;
using CPP_Metrics.FilesProcessing;
using CPP_Metrics.Metrics;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using Facads;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

public class Config
{
    public List<string> ProjectFiles { get; set; } = new();
    public string? OutReportPath { get; set; }
    public string? OutReportPathXml { get; set; }
    public HashSet<string> CompilerAddFiles { get; set; } = new();
    public string? ProjectName { get; set; }
    public FileInfo? BoundaryValues { get; set; }
}

class TestClass
{
    //-cp
    static void CreateProjects(string name)
    {
        using (var db = new DbContextMetrics())
        {
            var project = new Project() { Name = name, Date = DateTime.UtcNow };
            db.Projects.Add(project);
            db.SaveChanges();
            Console.WriteLine($"Created project succes id {project.ID}");
        }
    }
    //-gp
    static void GetProjects(string? name)
    {
        using (var db = new DbContextMetrics())
        {
            var table = new ConsoleTable("Id", "Name","Date");
            var projects = db.Projects.ToList();
            if(name is not null)
                projects = projects.Where(p => p.Name == name).ToList();
            
            if(projects is null)
            {
                Console.WriteLine("Not find projecs");
                return;
            }

            foreach (var item in projects)
            {
                table.AddRow(item.ID, item.Name,item.Date); 
            }
            
            table.Write();
        }
    }

    //-gs
    static void GetSolution(int projectId)
    {
        using (var db = new DbContextMetrics())
        {
            var table = new ConsoleTable("Id", "Date","ProjectName");
            var solutions = db.Solutions.Include(x=> x.Project).Where(x => x.ProjectID == projectId).ToList();
            if (solutions is null)
            {
                Console.WriteLine("Not find solutions");
                return;
            }
            foreach (var item in solutions)
            {
                table.AddRow(item.ID, item.Date,item.Project.Name);
            }

            table.Write();
        }
    }
    //-gm
    static void GetMetrics(int solutionId)
    {
        using (var db = new DbContextMetrics())
        {
            var table = new ConsoleTable("Id","Metric", "FileName", "ObjectName", "Value");
            var metricValues = db.MetricValues.Include(x => x.MetricDirectory).ThenInclude(x=> x.LevelMetric).Where(x => x.SolutionID == solutionId).ToList();
            if (metricValues is null)
            {
                Console.WriteLine("Not find metricsValues");
                return;
            }
            foreach (var item in metricValues)
            {
                table.AddRow(item.ID, item.MetricDirectory.Name,item.FileName,item.ObjectName,item.Value);
            }

            table.Write();
        }
    }

    static void UpdateProject(string name, string newName)
    {
        using (var db = new DbContextMetrics())
        {
            var project = db.Projects.FirstOrDefault(x => x.Name == name);
            if (project is null)
            {
                Console.WriteLine("Error! Update project name was not found");
                return;
            }
            project.Name = newName;
            db.SaveChanges();
        }
        
    }

    static void DeleteProject(string name)
    {
        using (var db = new DbContextMetrics())
        {
            var project = db.Projects.FirstOrDefault(x => x.Name == name);
            if (project is null)
            {
                Console.WriteLine("Error! Delete project name was not found");
                return;
            }
            db.Projects.Remove(project);
            db.SaveChanges();
        }
    }


    static public void HandleArguments(Config config, string[] args)
    {
        // -f C:\Users\User\Documents\interpreter\Interpreter -o C:\Users\User\Documents\interpreter\Interpreter
        for (int i = 0; i < args.Length;)
        {
            switch (args[i])
            {
                case "-cp":
                    i++;
                    string? newprjname = null;
                    if (i < args.Length && !args[i].StartsWith('-'))
                        newprjname = args[i];
                    CreateProjects(newprjname);
                    i++;
                    break;
                case "-gp":
                    i++;
                    string? prjname = null;
                    if (i < args.Length && !args[i].StartsWith('-'))
                        prjname = args[i];
                    GetProjects(prjname);
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
                    GetSolution(prjId);
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
                    GetMetrics(solId);
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
                    // Update
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected project name");
                    var name = args[i];
                    i++;
                    if (args[i].StartsWith('-') || i >= args.Length)
                        throw new Exception("Expected new project name");
                    var newName = args[i];
                    UpdateProject(name,newName);
                    i++;
                    break;
                case "-dp":
                    // Delete
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected project name");
                    var deleteName = args[i];
                    DeleteProject(deleteName);
                    i++;
                    break;
                case "-xmlo":
                    // Delete
                    i++;
                    if (i >= args.Length || args[i].StartsWith('-'))
                        throw new Exception("Expected path to xml report");
                    var xmlo = args[i];
                    config.OutReportPathXml = xmlo;
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
    private static ReportInfo PrepareReports(Config config)
    {
        if (config.OutReportPath is null)
            throw new Exception("Path to out report files is empty");

        var dirInfo = new DirectoryInfo(config.OutReportPath);
        var reportFilesPath = Path.Combine(dirInfo.FullName, "Report");
        var reportDirInf = new DirectoryInfo(reportFilesPath);
        if (!reportDirInf.Exists)
        {
            reportDirInf.Create();
        }
        var curdir = Directory.GetCurrentDirectory();
        try
        {
            File.Copy(Path.Combine(curdir, "7c8770672ebc45c18fbc3a5bdc3dd9b9.png"), reportFilesPath);
            File.Copy(Path.Combine(curdir, "main.css"), reportFilesPath);
        }
        catch (Exception)
        {
        }

        DateTime localDate = DateTime.Now;
        var culture = new CultureInfo("ru-RU");
        var folderName = localDate.ToString(culture);
        folderName = folderName.Replace(':', ' ');

        var currentReportFolderPath = Path.Combine(reportDirInf.FullName, folderName);
        var currentReportFolder = new DirectoryInfo(currentReportFolderPath);
        if (!currentReportFolder.Exists)
        {
            currentReportFolder.Create();
        }

        ReportInfo reportInfo = new ReportInfo();
        reportInfo.Header = RenderBody.Header;
        reportInfo.Footer = RenderBody.Footer;
        reportInfo.OutPath = currentReportFolderPath;
        return reportInfo;
    }
    public static List<MetricMessage> metricMessages= new List<MetricMessage>();
    private static void RunMetrics(List<string> soursePaths, Config config)
    {
        if (soursePaths.Count == 0)
            throw new Exception("Empty sourse path list");
        var reportInfo = PrepareReports(config);


        //var soursePaths = new List<string>() { @"C:\Users\User\source\repos\TestCpp1\TestCpp1" };
        ProcessingFile processingFile = new ProcessingFile(soursePaths,config);
        
        processingFile.ReportInfo = reportInfo;

        var SLocReport = new SlocReportBuilder(reportInfo);
        processingFile.Metrics.Add(new SLoc(SLocReport));

        var cyclomaticReport = new CyclomaticReportBuilder(reportInfo);
        processingFile.Metrics.Add(new CylomaticComplexity(cyclomaticReport));

        var classAbstractionBuilder = new AbstractReportBuilder(reportInfo);
        processingFile.Metrics.Add(new ClassAbstraction(classAbstractionBuilder));

        var DITReport = new DITReportBuilder(reportInfo);
        processingFile.Metrics.Add(new DIT(DITReport));

        var CBOReport = new CBOReportBuilder(reportInfo);
        processingFile.Metrics.Add(new CBOMetric(CBOReport));
        
        var CaCeReport = new CaCeReportBuilder(reportInfo);
        processingFile.Metrics.Add(new CaCeMetric(CaCeReport));
        
        // Комбинированные метрики
        var instabilityReport = new InstabilityReport(reportInfo);
        processingFile.CombineMetrics.Add(new InstabilityMetric(instabilityReport));
        
        processingFile.Run();
        metricMessages.AddRange(processingFile.MetricMessages);
    }
    static void TestRun()
    {
        string pathFile = "C:/Users/User/source/repos/TestCpp1/TestCpp1/TestCpp1.cpp";
        var facad = new Facad(new StreamReader(pathFile));

        var three = facad.GetTree();
        //BaseContextElement contextElement = BaseContextElement.GetGeneralNameSpace();
        var visitor = new TestVisitor();

        Analyzer.Analyze(three, visitor);

    }
    static void Main(string[] args)
    {
        Console.WriteLine("Cpp metrics running");
        
       
        
        foreach (var item in args)
        {
            Console.WriteLine($"arg {item}");
        }
        //TestRun();

        Config config = new Config();
        if (args.Length != 0)
        {
            HandleArguments(config, args);
            RunMetrics(config.ProjectFiles,config);
            var xmlpath = "C:/Users/User/Desktop";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                xmlpath = "/";
            }

            if (config.OutReportPathXml is not null)
                xmlpath = config.OutReportPathXml;

            XMLReport xMLReport = new(xmlpath, metricMessages);
            xMLReport.ReportBuild();

            Console.WriteLine("Cpp metrics finished");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\n======================================================================================");
            stringBuilder.AppendLine("                                     PROBLEMS                                           ");
            stringBuilder.AppendLine("======================================================================================");

            foreach (var item in metricMessages.Where(x=> x.MessageType == MessageType.Error)) {
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

        //C:\Users\User\source\repos\TestCpp1\TestCpp1
        //config.ProjectFiles.Add(@"C:\Users\User\Documents\interpreter\Interpreter");
        //config.OutReportPath = @"C:\Users\User\Documents\interpreter\Interpreter";

        string? command = "";
        while (!command.Equals("exit"))
        {
            Console.Write("CPP_Metrics>");
            command = Console.ReadLine();

            var commands = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < commands.Length;)
            {
                switch (command)
                {
                    case "exit":
                        return;
                    case "run":
                        RunMetrics(config.ProjectFiles, config);
                        Console.WriteLine("Press any key for exit");
                        Console.ReadLine();
                        return;
                    case "-i":
                        i++;
                        do
                        {
                            if (args[i].StartsWith('-') || i >= args.Length)
                                throw new Exception("Expected path to additional processor files");
                            config.ProjectFiles.Add(args[i]);
                            i++;
                        } while (!args[i].StartsWith('-') && i < args.Length);
                        break;
                    case "-f":
                        i++;
                        do
                        {
                            if (args[i].StartsWith('-') || i >= args.Length)
                                throw new Exception("Expected path to project files");
                            config.ProjectFiles.Add(args[i]);
                            i++;
                        } while (!args[i].StartsWith('-') && i < args.Length);

                        break;
                    case "-o":
                        i++;
                        if (args[i].StartsWith('-') || i >= args.Length)
                            throw new Exception("Expected path to out report files");
                        config.OutReportPath = args[i];
                        break;
                    case "-cfg":
                        break;
                    default:
                        throw new Exception($"Invalid parameter {args[i]}");

                }
            }
        }
        
    }

    
}


//return;

//void DisplayContext(BaseContextElement ContextElement)
//{
//    if(ContextElement is ClassStructDeclaration classStructDeclaration)
//    {
//        Console.WriteLine("-ClassInfo===========");
//        Console.WriteLine("-Fields");
//        foreach (var item in classStructDeclaration.ClassStructInfo.Fields)
//        {
//            Console.WriteLine(item.Name);
//        }
//        Console.WriteLine("-Methods");
//        foreach (var item in classStructDeclaration.ClassStructInfo.Methods)
//        {
//            Console.WriteLine(item.Name);
//        }
//    }

//    Console.WriteLine(ContextElement.GetType());
//    if(ContextElement is NamespaceContext namespaceContext)
//        Console.WriteLine(namespaceContext.NameSpaceInfo.Name);
//    Console.WriteLine("-VariableNames");
//    foreach (var variable in ContextElement.VariableDeclaration)
//        Console.WriteLine($"----type:{variable.Value.Type?.TypeName},name:{variable.Value.Name}");
//    Console.WriteLine("-DeclFuncNames");
//    foreach (var name in ContextElement.FunctionDeclaration)
//        foreach (var item in name.Value)
//        {
//            Console.WriteLine(item.Name);
//        }
//    Console.WriteLine("-TypeName");
//    foreach (var item in ContextElement.TypeDeclaration)
//        Console.WriteLine($"----Name: {item.Value.Name}");
//}

//string pathFile = "C:/Users/User/source/repos/TestCpp1/TestCpp1/TestCpp1.cpp";

//var facad = new Facad(new StreamReader(pathFile));

//var three = facad.GetTree();

//var generalVisitor = new GeneralVisitor();
//var variableVisitor = new VariableVisitor();
//var classVisitor = new ClassStructVisitor();
//var typeVisitor = new TypeVisitor();


////Analyzer.Analyze(three, generalVisitor);

////TODO: Разобраться с аналайзером для Цикломатической(!там нужно ходить справа налево)
//BaseContextElement ContextElement = BaseContextElement.GetGeneralNameSpace();

//var contextVisitor = new GlobalContextVisitor(ContextElement);
//Analyzer.Analyze(three, contextVisitor);

//DisplayContext(ContextElement);

//foreach (var item in ContextElement.Children)
//{
//    DisplayContext(item);
//}


//Console.WriteLine();