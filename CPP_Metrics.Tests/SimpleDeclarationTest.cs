
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using Facads;

namespace CPP_Metrics.Tests
{
    public class SimpleDeclarationTest
    {
		[Fact]
		public void Simple()
		{
			var code = @"""
            int main()
			{
				int a;
				a = 2;
			}
            """; 
			
			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var visitor = new GlobalContextVisitor(generalNamespase);

			Analyzer.Analyze(tree, visitor);

			Assert.True(generalNamespase.Children.Count == 1);
			var func = generalNamespase.Children.First();
			Assert.True(func is FunctionDeclaration);
			var funcContext = (FunctionDeclaration)func;

			Assert.True(funcContext.VariableDeclaration.Count == 1);
			var variable = funcContext.VariableDeclaration.First().Value;


			Assert.True(variable.References.Count == 1);
			Assert.True(variable.References.First() == funcContext);
		}
		[Fact]
		public void Simple2()
		{
			var code = @"""
			int a;
            int main()
			{
				a = 2;
			}
            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var visitor = new GlobalContextVisitor(generalNamespase);

			Analyzer.Analyze(tree, visitor);

			Assert.True(generalNamespase.Children.Count == 1);
			var func = generalNamespase.Children.First();
			Assert.True(func is FunctionDeclaration);
			var funcContext = (FunctionDeclaration)func;

			Assert.True(generalNamespase.VariableDeclaration.Count == 1);
			var variable = generalNamespase.VariableDeclaration.First().Value;


			Assert.True(variable.References.Count == 1);
			Assert.True(variable.References.First() == funcContext);

		}
        [Fact]
		public void Simple3()
		{
			var code = @"""
			class cl
			{
				int a;
				void foo()
				{
					a = 2;
				}
			};
            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var visitor = new GlobalContextVisitor(generalNamespase);

			Analyzer.Analyze(tree, visitor);

			Assert.True(generalNamespase.Children.Count == 1);
			var context = generalNamespase.Children.First();
			
			Assert.True(context is ClassStructDeclaration);
			var classContext = (ClassStructDeclaration)context;

			Assert.True(classContext.Children.Count == 1);
			var methodContext = classContext.Children.First();

			Assert.True(methodContext is FunctionDeclaration);
			var method = (FunctionDeclaration)methodContext;

			Assert.True(classContext.ClassStructInfo.Fields.Count == 1);
			var variable = (Variable)classContext.ClassStructInfo.Fields.First();


			Assert.True(variable.References.Count == 1);
			Assert.True(variable.References.First() == method);

		}
		[Fact]
		public void Simple4()
		{
			var code = @"""
			class cl
			{
				int a;
				void foo()
				{
					this.a = 2;
				}
			};
            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var visitor = new GlobalContextVisitor(generalNamespase);

			Analyzer.Analyze(tree, visitor);

			Assert.True(generalNamespase.Children.Count == 1);
			var context = generalNamespase.Children.First();

			Assert.True(context is ClassStructDeclaration);
			var classContext = (ClassStructDeclaration)context;

			Assert.True(classContext.Children.Count == 1);
			var methodContext = classContext.Children.First();

			Assert.True(methodContext is FunctionDeclaration);
			var method = (FunctionDeclaration)methodContext;

			Assert.True(classContext.ClassStructInfo.Fields.Count == 1);
			var variable = (Variable)classContext.ClassStructInfo.Fields.First();


			Assert.True(variable.References.Count == 1);
			Assert.True(variable.References.First() == method);

		}
	}
}
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

	int ((f3)());
	foo1(a);
	foo1(foo());
	
	foo2(foo(), a);
	foo2(foo(),1);
	foo2(a,b);
	foo2(a,1);
	

	
	int (*a1) = &a;
	int a2(1);
	int a22(int);
	int a3{1};
	int a4((int)a2);
	int (a5)(a2);
	
	int (f)();

	int f1();
	int (f2)();
	return 0;
}
"""
*/