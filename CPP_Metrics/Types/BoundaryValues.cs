namespace CPP_Metrics.Types
{
    public class BoundaryValues
    {
        public int Complexity { get; set; } = 12;
        public int DIT { get; set; } = 5;
        public int CBO { get; set; } = 15;
        public int CA { get; set; } = 100;
        public int CE { get; set; } = 100;
        public int NOC { get; set; } = 100;
        public int PercentCommented { get; set; } = 25;
        public int PercentEmpty { get; set; } = 25;
        public decimal RadiusPain { get; set; } = 0.4m;
        public decimal RadiusFutility { get; set; } = 0.4m;
    }
}
