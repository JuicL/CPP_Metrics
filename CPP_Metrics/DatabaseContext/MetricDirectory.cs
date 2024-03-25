namespace CPP_Metrics.DatabaseContext
{
    public class MetricDirectory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LevelMetricID {get;set; }
        public LevelMetric LevelMetric { get; set; }
    }
}
