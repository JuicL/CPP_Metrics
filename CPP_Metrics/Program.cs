
using CPP_Metrics;
using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.OOP;
using CPP_Metrics.Tool;
using Facads;

string pathFile = "C:/Users/User/source/repos/TestCpp1/TestCpp1/TestCpp1.cpp";

var facad = new Facad(new StreamReader(pathFile));

var three = facad.GetTree();

var generalVisitor = new GeneralVisitor();
var variableVisitor = new VariableVisitor();
var classVisitor = new ClassStructVisitor();
var typeVisitor = new TypeVisitor();



//TODO: Разобраться с аналайзером для Цикломатической(!там нужно ходить справа налево)
Analyzer.Analyze(three, generalVisitor);

Console.WriteLine();