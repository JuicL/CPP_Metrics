using CPP_Metrics.Tool;

namespace CPP_Metrics.FilesPrepare
{
    /// <summary>
    /// Get all files in specified directiry, with specified extensions 
    /// </summary>
    public class DirectoiryFiles
    {
        public static List<FileInfo> GetFiles(List<string> sourcePath, string[] extensions)
        {
            List<FileInfo> files = new();
            foreach (string path in sourcePath)
            {
                Stack<DirectoryInfo> directories = new();
                DirectoryInfo dInfo = new DirectoryInfo(path);
                directories.Push(dInfo);

                while (directories.Any())
                {
                    var currentDir = directories.Pop();
                    files.AddRange(currentDir.GetFilesByExtensions(extensions));
                    var innerDir = currentDir.GetDirectories();
                    foreach (var dir in innerDir)
                    {
                        directories.Push(dir);
                    }
                }
            }
            return files;
        }
            
    }
}
