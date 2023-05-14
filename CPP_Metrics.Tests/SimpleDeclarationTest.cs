using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using Facads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPP_Metrics.Tests
{
    public class SimpleDeclarationTest
    {
		/*

"""
int foo();
int foo1(int);
int foo2(int, int);

int foo() { return 1; }
int foo1(int) { return 1; }
int foo2(int, int) { return 1; }

int main()
{
	int a = 5;
	fff(M1(a));
	int b = 5;

	
	foo1(a);
	foo1(foo());
	
	foo2(foo(), a);
	foo2(foo(),1);
	foo2(a,b);
	foo2(a,1);
	

	int (f)();
	int f1();
	int f2(int);
	int ((f3)());
	int (f4)();

	return 0;
}
"""
*/
		[Fact]
		public void VariableDeclaration()
        {
			var code = @"""
			int main()
			{
				int (*a1);
				int a2(1);
				int a3{1};
				int a4((int)a2);
				int (a5)(a22);
			}
            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a22",new Variable() { Name = "a22"});

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);

			Assert.True(visitor.VariablesDeclaration.Count == 5);
		}
		[Fact]
		public void FuncDeclaration()
		{
			var code = @"""
			int main()
			{
				int (f)();
				int f1();
				int f2(int);
				int ((f3)());
				int (f4)();
			}
            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);

			Assert.True(visitor.FunctionDeclaration.Count == 5);
		}
		[Fact]
		public void CallFunc1()
		{
			var code = @"""
			int main()
			{
				foo1(a);
			}
            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });
			generalNamespase.FunctionDeclaration.Add("foo1", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo1" } });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);

			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "foo1");
		}
		[Fact]
		public void CallFunc2()
		{
			var code = @"""
			int main()
			{
				foo1(foo());
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.FunctionDeclaration.Add("foo1", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo1" } });
			generalNamespase.FunctionDeclaration.Add("foo", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo" } });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "foo1");
		}
		[Fact]
		public void CallFunc3()
		{
			var code = @"""
			int main()
			{
				foo2(foo(),1);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.FunctionDeclaration.Add("foo2", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo2" } });
			generalNamespase.FunctionDeclaration.Add("foo", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo" } });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "foo2");
		}
		[Fact]
		public void CallFunc4()
		{
			var code = @"""
			int main()
			{
				foo2(a,b);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });
			generalNamespase.VariableDeclaration.Add("b", new Variable() { Name = "b" });

			generalNamespase.FunctionDeclaration.Add("foo2", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo2" } });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "foo2");
		}
		[Fact]
		public void CallFunc5()
		{
			var code = @"""
			int main()
			{
				foo2(a,1);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });

			generalNamespase.FunctionDeclaration.Add("foo2", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo2" } });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "foo2");
		}
		[Fact]
		public void CallFunc6()
		{
			var code = @"""
			int main()
			{
				foo2(foo(), a);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });

			generalNamespase.FunctionDeclaration.Add("foo2", new List<FunctionInfo>() { new FunctionInfo() { Name = "foo2" } });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "foo2");
		}
		[Fact]
		public void CallFunc7()
		{
			var code = @"""
			int main()
			{
				cl::Foo();
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });


			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "Foo");
		}
		[Fact]
		public void CallFunc8()
		{
			var code = @"""
			int main()
			{
				cl::Foo(a);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "Foo");
		}
		[Fact]
		public void CallFunc9()
		{
			var code = @"""
			int main()
			{
					cl::Foo(1);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			// Additional iformation

			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			Assert.True(visitor.CallFuncNames.Count == 1);
			Assert.True(visitor.CallFuncNames.First() == "Foo");
		}
		[Fact]
		public void CallFunc10()
		{
			var code = @"""
			int main()
			{
					NN::cl(a);
			}
            """;
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var ns = new NamespaceContext() { ParenNameSpace = (NamespaceContext)generalNamespase};
			ns.NameSpaceInfo = new NameSpaceInfo() { Name = "NN",
				Nested = new List<CPPType>() { new CPPType() { TypeName ="::"}} };
			
			ns.Paren = generalNamespase;
			var classStuct = new ClassStructDeclaration();
			classStuct.ClassStructInfo = new ClassStructInfo() { Name = "cl" };
			ns.Children.Add(classStuct);
			classStuct.Paren = ns;
			generalNamespase.Children.Add(ns);

			// Additional iformation
			generalNamespase.VariableDeclaration.Add("a", new Variable() { Name = "a" });
			var visitor = new SimpleDeclarationContextVisitor(generalNamespase);
			Analyzer.AnalyzeWithСondition(tree, visitor, x => x is CPP14Parser.SimpleDeclarationContext);
			
		}
	}
}
