
using CPP_Metrics.Tool;
using CPP_Metrics.Types.Context;
using Facads;

namespace CPP_Metrics.Tests
{
    public class SimpleDeclarationContextTest
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
		[Fact]
		public void Simple5()
		{
			var code = @"""
			class ArifParser
			{
				int input;
			};

			Expression ArifParser::parse_binary_expression(int min_priority) {
				for (;;) {
					if (priority <= min_priority) {
						input -= op.size();
						return left_expr;
					}

					auto right_expr = parse_binary_expression(priority);
					left_expr = Expression(op, left_expr, right_expr);
				}
			}

            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var visitor = new GlobalContextVisitor(generalNamespase);

			Analyzer.Analyze(tree, visitor);

			Assert.True(generalNamespase.Children.Count == 2);
			var context = generalNamespase.Children.FirstOrDefault(x => x is ClassStructDeclaration);

			Assert.True(context is not null);
			var classContext = (ClassStructDeclaration)context;

			

			var methodContext = generalNamespase.Children.FirstOrDefault(x => x is FunctionDeclaration);

			Assert.True(methodContext is not null);
			var method = (FunctionDeclaration)methodContext;

			Assert.True(classContext.ClassStructInfo.Fields.Count == 1);
			var variable = (Variable)classContext.ClassStructInfo.Fields.First();


			Assert.True(variable.References.Count == 1);
			//Assert.True(variable.References.First() == method);


		}
		[Fact]
		public void Simple6()
		{
			var code = @"""
			namespace N1
			{
				class CL1{};
			}

			namespace N2 {
				using namespace N1;
				class CL2 {
		
				};
			}

            """;

			var facad = new Facad(code);
			var tree = facad.GetTree();
			var generalNamespase = BaseContextElement.GetGeneralNameSpace();
			var visitor = new GlobalContextVisitor(generalNamespase);

			Analyzer.Analyze(tree, visitor);

			Assert.True(generalNamespase.Children.Count == 2);
			var context = generalNamespase.Children.LastOrDefault(x => x is NamespaceContext);

			Assert.True(context is not null);
			var namespaceContext = (NamespaceContext)context;

			var class1 = namespaceContext.Children.FirstOrDefault(x => x is ClassStructDeclaration);
			Assert.True(class1 is not null);

			var t = class1.GetTypeName("CL1", new List<CPPType>());
			Assert.True(t is not null);


		}
	}
}
