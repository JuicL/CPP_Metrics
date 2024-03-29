using CPP_Metrics.Metrics.CyclomaticComplexity;
using Facads;

namespace CPP_Metrics.Tests
{
    public class CyclomaticComplexityTest
    {
        [Fact]
        public void OneIf()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {

                }
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 2;
            Assert.True(result);
        }
        [Fact]
        public void OneIf2()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true || a == false)
                {

                }
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 3;
            Assert.True(result);
        }
        [Fact]
        public void tt()
        {
            string text = """
            void TestFoo()
            {
                bool a = (y < 10) ? 30 : 40;
               
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 2;
            Assert.True(result);
        }
        [Fact]
        public void tt2()
        {
            string text = """
            void TestFoo()
            {
                bool a = (y < 10 || y < 10 ) ? 30 : 40;
               
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 3;
            Assert.True(result);
        }
        [Fact]
        public void OneIfElse()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {

                }
                else
                {
                }
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 2;
            Assert.True(result);
        }
        [Fact]
        public void DoubleIf()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {

                }
                if (a == false)
                {

                }
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 3;
            Assert.True(result);
        }
        [Fact]
        public void IfInnerIf()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {
                    if (a == false)
                    {

                    }
                }
                
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 3;
            Assert.True(result);
        }
        [Fact]
        public void IfElseInnerIf()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {
                    if (a == false)
                    {

                    }
                    else
                    {
                    }
                }
                
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();

            var result = func.CyclomaticComplexityValue == 3;
            Assert.True(result);
        }
        [Fact]
        public void IfElseIf()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {
                    
                } else if(a == false)
                {}
                
                
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();
            var cyclomaticResult = func.CyclomaticComplexityValue;
            var result = cyclomaticResult == 3;
            Assert.True(result);
        }
        [Fact]
        public void IfElseIfElseIf()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {
                    
                } else if(a == false)
                {}
                else if(a == false)
                {}
                
                
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();
            var cyclomaticResult = func.CyclomaticComplexityValue;
            var result = cyclomaticResult == 4;
            Assert.True(result);
        }
        [Fact]
        public void IfWhile()
        {
            string text = """
            void TestFoo()
            {
                bool a = true;
                if (a == true)
                {
                    
                } 
                while(a == false)
                {
                }
                
                
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();
            var cyclomaticResult = func.CyclomaticComplexityValue;
            var result = cyclomaticResult == 3;
            Assert.True(result);
        }
        [Fact]
        public void For()
        {

        }
        [Fact]
        public void DoWhile()
        {

        }
        //string text = """
        //    void TestFoo()
        //    {
        //        int a = 0;
        //        switch ( c )
        //        {
        //           case 1:
        //              break;
        //           case 2:
        //              break;
        //           default:
        //              break;
        //        }
                
                
        //    }
        //    """
        [Fact]
        public void Switch()
        {
            string text = """
            void TestFoo()
            {
                int a = 0;
                switch ( a )
                {
                   case 1:
                      break;
                   default:
                       break;
                }
                
                
            }
            """;

            CyclomaticComplexityMetric metric = new();
            var facad = new Facad(text);
            var tree = facad.GetTree();
            metric.Analyze(tree);
            var func = metric.Cyclomatic.LastOrDefault();
            var cyclomaticResult = func.CyclomaticComplexityValue;
            var result = cyclomaticResult == 3;
            Assert.True(result);
        }
        [Fact]
        public void Break_Return_Continue()
        {

        }
        [Fact]
        public void HardExpression()
        {

        }
    }
}