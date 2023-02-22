

namespace CPP_Metrics.Types.DIT
{
    public class DITEdge : IEdge<DITVertex>
    {
        public DITVertex From { get; set; }
        public DITVertex To { get; set; }
        public decimal Price { get; set; }
    }
}
