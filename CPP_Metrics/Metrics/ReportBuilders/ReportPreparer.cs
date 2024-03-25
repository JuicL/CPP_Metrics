using System.Globalization;

namespace CPP_Metrics.Metrics.ReportBuilders
{
    public class ReportPreparer
    {
        public static ReportInfo Prepare(Config config)
        {
            if (config.OutReportPath is null)
                throw new Exception("Path to out report files is empty");

            var dirInfo = new DirectoryInfo(config.OutReportPath);
            var reportFilesPath = Path.Combine(dirInfo.FullName, "Report");
            var reportDirInf = new DirectoryInfo(reportFilesPath);
            if (!reportDirInf.Exists)
            {
                reportDirInf.Create();
            }
            var curdir = Directory.GetCurrentDirectory();
            try
            {
                File.Copy(Path.Combine(curdir, "7c8770672ebc45c18fbc3a5bdc3dd9b9.png"), reportFilesPath);
                File.Copy(Path.Combine(curdir, "main.css"), reportFilesPath);
            }
            catch (Exception)
            {
            }

            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("ru-RU");
            var folderName = localDate.ToString(culture);
            folderName = folderName.Replace(':', ' ');

            var currentReportFolderPath = Path.Combine(reportDirInf.FullName, folderName);
            var currentReportFolder = new DirectoryInfo(currentReportFolderPath);
            if (!currentReportFolder.Exists)
            {
                currentReportFolder.Create();
            }

            ReportInfo reportInfo = new ReportInfo();
            reportInfo.Header = RenderBody.Header;
            reportInfo.Footer = RenderBody.Footer;
            reportInfo.OutPath = currentReportFolderPath;
            return reportInfo;
        }
    }
}
