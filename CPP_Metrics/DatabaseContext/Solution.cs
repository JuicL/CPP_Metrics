namespace CPP_Metrics.DatabaseContext
{
    public class Solution
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public DateTime Date { get; set; }
        public Project Project { get; set; }
    }
}
