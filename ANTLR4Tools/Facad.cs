using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Facads
{
    public class Facad
    {
        public Facad() { }
        public Facad(StreamReader streamReader) => StreamReader = streamReader;
        public Facad(string text) => Text = text;

        private readonly StreamReader StreamReader;

        public readonly string Text = "";
        public CPP14Lexer Lexer { get; private set; }
        public CPP14Parser Parser { get; private set; }

        
        public IParseTree GetTree()
        {
            AntlrInputStream antlrInputStream;
            
            if(StreamReader != null)
            {
                antlrInputStream = new AntlrInputStream(StreamReader);
            }
            else
            {
                antlrInputStream = new AntlrInputStream(Text);
            }   
            
            Lexer = new CPP14Lexer(antlrInputStream);

            CommonTokenStream tokens = new CommonTokenStream(Lexer);
            StringWriter output = new StringWriter();
            StringWriter errorOutput = new();
            Parser = new CPP14Parser(tokens, output, errorOutput);

            IParseTree tree = Parser.translationUnit();

            return tree;
        }
    }
}
