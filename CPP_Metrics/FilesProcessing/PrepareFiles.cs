

using Antlr4.Runtime.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace CPP_Metrics.FilesPrepare
{
    public class PrepareFiles
    {
        public string[] Extentions { get; } = new []{ ".cpp", ".h", ".hpp" };
        public string[] CongigExtentions { get; } = new[] { ".cppconfig" };

        public List<string> SourceFilesPath { get; } = new List<string>();
        public Dictionary<string, FileInfo> Files { get; } = new();
        public Dictionary<string, FileInfo> ConfigFiles { get; } = new();
        public Config Config { get; set; }
        public List<Regex> FilesRegex { get; } = new();

        public string PathToTempFilesDirectory { get; }
        public PrepareFiles(List<string> sourceFilesPath, Config config)
        {
            Config = config;
            SourceFilesPath = sourceFilesPath;
            GetFileProject();
            AddPathToProjectFolders();

            PathToTempFilesDirectory = CreateDirectoryForTempFiles();
            foreach (var source in SourceFilesPath)
            {
                var regexString = source.Replace("\\", ".+").Replace("/",".+");
                Regex regex = new Regex(regexString);
                FilesRegex.Add(regex);
            }
            GetConfigFiles();
        }
        private void AddPathToProjectFolders()
        {
            foreach (var file in Files)
            {
                Config.CompilerAddFiles.Add(file.Value.DirectoryName);
            }

        }
        private void GetConfigFiles()
        {
            var files = DirectoiryFiles.GetFiles(SourceFilesPath, CongigExtentions);
            foreach (var item in files)
            {
                ConfigFiles.Add(item.FullName, item);
            }
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
            var outFile = Path.Combine(PathToTempFilesDirectory, 
                filePath.Name + ".ii");
            //var test = $"g++ -E {filePath.FullName} -o {outFile}";
            ProcessStartInfo startInfo;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                StringBuilder command = new($"-E {filePath.FullName} -o {outFile} ");
                foreach (var item in Config.CompilerAddFiles)
                {
                    command.Append(" -I " + item);
                }

                startInfo = new ProcessStartInfo() {
                    FileName = "g++",
                    UseShellExecute = true,
                    Arguments = command.ToString(), };
            }
            else
            {
                StringBuilder command = new($"g++ -E {filePath.FullName} -o {outFile} ");
                foreach (var item in Config.CompilerAddFiles)
                {
                    command.Append(" -I " + item);
                }
                var test = Path.GetDirectoryName(filePath.FullName);
                startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    UseShellExecute = true,
                    Arguments = $"/C cd {Path.GetDirectoryName(filePath.FullName)} && {command.ToString()} && exit",
                    WindowStyle = ProcessWindowStyle.Hidden
                };
            }
            

            Process process = new Process();
            process.StartInfo = startInfo;
            //process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            //string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return outFile;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="preprocessedFilePath"></param>
        /// <returns>Pair include preprocesser file path and current file after preprocessed processing</returns>
        /// <exception cref="Exception"></exception>
        public Pair<string,string> ReadPreprocessedFile(FileInfo fileInfo,string preprocessedFilePath)
        {
            var fileName = fileInfo.Name;
            if (!File.Exists(preprocessedFilePath))
                throw new Exception("Preprocessed file not created");

            var fileIncludesPath = Path.Combine(PathToTempFilesDirectory, fileName + "_I.m");
            var filePath = Path.Combine(PathToTempFilesDirectory, fileName + "_P.m");

            FileStream fileIncludesFS = new FileStream(fileIncludesPath, FileMode.Create);
            StreamWriter fileIncludesWriter = new StreamWriter(fileIncludesFS, Encoding.Default);
            
            FileStream fileFS = new FileStream(filePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileFS, Encoding.Default);
            bool selector = false;
            bool isProhectFile = false;

            var regexString = fileInfo.FullName.Replace("\\", ".+").Replace("/",".+"); // Bruh s-word
            Regex regex = new Regex(regexString);

            foreach (string line in File.ReadLines(preprocessedFilePath))
            {
                if (line.StartsWith("#"))
                {
                    foreach (var item in FilesRegex)
                    {
                        var matches = item.Matches(line);

                        isProhectFile = matches.Count > 0;
                        if (isProhectFile)
                        {
                            selector = regex.IsMatch(line);
                            break;
                        }

                    }
                    continue;
                }
                if (line.Length == 0)
                    continue;
                if (isProhectFile == false)
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
            return new Pair<string, string>(fileIncludesPath, filePath);
        }
    }
}