

namespace CPP_Metrics.FilesPrepare
{
    public class PrepareFiles
    {
        public string[] Extentions { get; } = new []{ ".cpp", ".h", ".hpp" };
        public List<string> SourceFilesPath { get; } = new List<string>();
        public Dictionary<string, FileInfo> Files { get; }
        public PrepareFiles(List<string> sourceFilesPath)
        {
            SourceFilesPath = sourceFilesPath;
            GetFileProject();
        }
        private void GetFileProject()
        {
            var files = DirectoiryFiles.GetFiles(SourceFilesPath, Extentions);
            foreach (var item in files)
            {
                Files.Add(item.FullName,item);
            }
        }

        public void HandleFile()
        {

        }
    }
}
