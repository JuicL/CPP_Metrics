namespace CPP_Metrics.Types
{
    public class MetricMessage
    {
        public MessageType MessageType { get; set; }
        public string Id { get; set; }
        public string Severity { get; set; } = "warning";
        // Подробнее
        public string Verbose { get; set; } = "";

        public string Message { get; set; }
    }

}
