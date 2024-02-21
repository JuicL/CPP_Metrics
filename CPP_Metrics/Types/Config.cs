public class Config
{
    public List<string> ProjectFiles { get; set; } = new();
    public string? OutReportPath { get; set; }
    public string? OutReportPathXml { get; set; }
    public HashSet<string> CompilerAddFiles { get; set; } = new();
    public string? ProjectName { get; set; }
    public FileInfo? BoundaryValues { get; set; }
}
