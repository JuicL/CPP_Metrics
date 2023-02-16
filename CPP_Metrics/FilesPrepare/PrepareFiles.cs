

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CPP_Metrics.FilesPrepare
{
    public class PrepareFiles
    {
        public string[] Extentions { get; } = new []{ ".cpp", ".h", ".hpp" };
        public List<string> SourceFilesPath { get; } = new List<string>();
        public Dictionary<string, FileInfo> Files { get; } = new();
        public string PathToTempFiles { get; }
        public PrepareFiles(List<string> sourceFilesPath)
        {
            SourceFilesPath = sourceFilesPath;
            GetFileProject();
            PathToTempFiles = CreateDirectoryForTempFiles();
        }
        private void GetFileProject()
        {
            var files = DirectoiryFiles.GetFiles(SourceFilesPath, Extentions);
            foreach (var item in files)
            {
                Files.Add(item.FullName,item);
            }
        }
        public static string CreateDirectoryForTempFiles()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dirInfo = new DirectoryInfo(currentDirectory);
            var tempFilesPath = Path.Combine(dirInfo.FullName, "TempFiles");
            var tempDirInf = new DirectoryInfo(tempFilesPath);
            if(!tempDirInf.Exists)
            {
                tempDirInf.Create();
            }
            return tempFilesPath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Preprocesed file</returns>
        public string CreatePreprocessorFile(FileInfo filePath)
        {
            var outFile = Path.Combine(PathToTempFiles, 
                Path.GetFileNameWithoutExtension(filePath.Name) + ".ii");
            var test = $"g++ -E {filePath.FullName} -o {outFile}";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = true,
                Arguments = $"/C g++ -E {filePath.FullName} -o {outFile} && exit",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process();
            process.StartInfo = startInfo;
            //process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            //string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outFile;
        }

        public void ReadPreprocessedFile(FileInfo fileInfo,string preprocessedFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            if (!File.Exists(preprocessedFilePath))
                throw new Exception("Preprocessed file not created");

            var fileIncludesPath = Path.Combine(PathToTempFiles, fileName + "_I.m");
            var filePath = Path.Combine(PathToTempFiles, fileName + "_P.m");

            FileStream fileIncludesFS = new FileStream(fileIncludesPath, FileMode.Create);
            StreamWriter fileIncludesWriter = new StreamWriter(fileIncludesFS, Encoding.Default);
            
            FileStream fileFS = new FileStream(filePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileFS, Encoding.Default);
            bool selector = false;
            var regexString = fileInfo.FullName.Replace("\\", ".+"); // Bruh dont this s-word
            Regex regex = new Regex(regexString);

            foreach (string line in File.ReadLines(preprocessedFilePath))
            {
                if(line.StartsWith("#"))
                {
                    var matches = regex.Matches(line);
                    if (matches.Count > 0)
                    {
                        selector = true;
                    }
                    else
                    {
                        selector = false;
                    }
                    continue;
                }
                if (line.Count() == 0)
                    continue;

                if (selector)
                {
                    fileWriter.WriteLine(line);
                }
                else
                {
                    fileIncludesWriter.WriteLine(line);
                }

            }
            fileIncludesWriter.Close();
            fileWriter.Close();
            fileIncludesFS.Close();
            fileFS.Close();
        }
    }
}