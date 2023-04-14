﻿
using CPP_Metrics;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.FilesPrepare;
using CPP_Metrics.FilesProcessing;
using CPP_Metrics.Metrics;
using CPP_Metrics.Metrics.ReportBuild;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using Facads;
using System.Globalization;

public class Config
{
    public List<string> ProjectFiles { get; set; } = new();
    public string OutReportPath { get; set; }
    public List<string> CompilerAddFiles { get; set; } = new();
}

class TestClass
{
    static public void HandleArguments(Config config, string[] args)
    {
        // -f C:\Users\User\Documents\interpreter\Interpreter -o C:\Users\User\Documents\interpreter\Interpreter
        for (int i = 0; i < args.Length;)
        {
            switch (args[i])
            {
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
                    i++;
                    break;
                case "-cfg":
                    break;
                default:
                    throw new Exception($"Invalid parameter {args[i]}");

            }
        }
    }
    private static ReportInfo PrepareReports(Config config)
    {
        var dirInfo = new DirectoryInfo(config.OutReportPath);
        var reportFilesPath = Path.Combine(dirInfo.FullName, "Report");
        var reportDirInf = new DirectoryInfo(reportFilesPath);
        if (!reportDirInf.Exists)
        {
            reportDirInf.Create();
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
    private static void RunMetrics(List<string> soursePaths, Config config)
    {
        if (soursePaths.Count == 0)
            throw new Exception("Empty sourse path list");
        var reportInfo = PrepareReports(config);


        //var soursePaths = new List<string>() { @"C:\Users\User\source\repos\TestCpp1\TestCpp1" };
        ProcessingFile processingFile = new ProcessingFile(soursePaths);
        processingFile.ReportInfo = reportInfo;
        
        var SLocReport = new SlocReportBuilder(reportInfo);
        processingFile.Metrics.Add(new SLoc(SLocReport));

        var cyclomaticReport = new CyclomaticReportBuilder(reportInfo);
        processingFile.Metrics.Add(new CylomaticComplexity(cyclomaticReport));

        var DITReport = new DITReportBuilder(reportInfo);
        processingFile.Metrics.Add(new DIT(DITReport));

        var classAbstractionBuilder = new AbstractReportBuilder(reportInfo);
        processingFile.Metrics.Add(new ClassAbstraction(classAbstractionBuilder));

        processingFile.Run();
    }
    static void Main(string[] args)
    {
        Config config = new Config();
        if (args.Length != 0)
        {
            HandleArguments(config, args);
            RunMetrics(config.ProjectFiles,config);
            return;
        }
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