
using CPP_Metrics;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.FilesPrepare;
using CPP_Metrics.FilesProcessing;
using CPP_Metrics.Metrics;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using Facads;

//"C:/Users/User/Desktop/folder3"

//PrepareFiles prepareFiles = new PrepareFiles(new List<string>() { @"C:\Users\User\Desktop\folder3\tt" });
//var FF = prepareFiles.Files.First().Value;
//var ppP = prepareFiles.CreatePreprocessorFile(FF);
//prepareFiles.ReadPreprocessedFile(FF, ppP);
//return;
var soursePaths = new List<string>() { @"" };
ProcessingFile processingFile = new ProcessingFile(soursePaths);
processingFile.Metrics.Add(new CylomaticComplexity());

return;

void DisplayContext(BaseContextElement ContextElement)
{
    if(ContextElement is ClassStructDeclaration classStructDeclaration)
    {
        Console.WriteLine("-ClassInfo===========");
        Console.WriteLine("-Fields");
        foreach (var item in classStructDeclaration.ClassStructInfo.Fields)
        {
            Console.WriteLine(item.Name);
        }
        Console.WriteLine("-Methods");
        foreach (var item in classStructDeclaration.ClassStructInfo.Methods)
        {
            Console.WriteLine(item.Name);
        }
    }

    Console.WriteLine(ContextElement.GetType());
    if(ContextElement is NamespaceContext namespaceContext)
        Console.WriteLine(namespaceContext.NameSpaceInfo.Name);
    Console.WriteLine("-VariableNames");
    foreach (var variable in ContextElement.VariableDeclaration)
        Console.WriteLine($"----type:{variable.Value.Type?.TypeName},name:{variable.Value.Name}");
    Console.WriteLine("-DeclFuncNames");
    foreach (var name in ContextElement.FunctionDeclaration)
        foreach (var item in name.Value)
        {
            Console.WriteLine(item.Name);
        }
    Console.WriteLine("-TypeName");
    foreach (var item in ContextElement.TypeDeclaration)
        Console.WriteLine($"----Name: {item.Value.Name}");
}

string pathFile = "C:/Users/User/source/repos/TestCpp1/TestCpp1/TestCpp1.cpp";

var facad = new Facad(new StreamReader(pathFile));

var three = facad.GetTree();

var generalVisitor = new GeneralVisitor();
var variableVisitor = new VariableVisitor();
var classVisitor = new ClassStructVisitor();
var typeVisitor = new TypeVisitor();


//Analyzer.Analyze(three, generalVisitor);

//TODO: Разобраться с аналайзером для Цикломатической(!там нужно ходить справа налево)
BaseContextElement ContextElement = BaseContextElement.GetGeneralNameSpace();

var contextVisitor = new GlobalContextVisitor(ContextElement);
Analyzer.Analyze(three, contextVisitor);

DisplayContext(ContextElement);

foreach (var item in ContextElement.Children)
{
    DisplayContext(item);
}


Console.WriteLine();