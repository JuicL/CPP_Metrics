

using Antlr4.Runtime.Tree;

namespace CPP_Metrics.Types
{
    public class ProcessingFileInfo
    {
        public string IncludeFile { get; set; }
        public string IncludeFilePath { get; set; }
        public IParseTree IncludeFileTree { get; set; }

        public string ProcessingFile { get; set; }
        public string ProcessingFilePath { get; set; }
        public IParseTree ProcessingFileTree { get; set; }

    }
}
