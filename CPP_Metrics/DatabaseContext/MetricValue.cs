namespace CPP_Metrics.DatabaseContext
{
    public class MetricValue
    {
        public int ID { get; set; }
        //Название файла
        public string FileName { get; set; }
        // Название объекта
        public string ObjectName { get; set; }
        public int MetricDirectoryID { get; set; }
        public int SolutionID { get; set; }
        public decimal Value { get; set; }
        public Solution Solution { get; set; }
        public MetricDirectory MetricDirectory { get; set; }
    }
}
