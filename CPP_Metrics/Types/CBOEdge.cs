using CPP_Metrics.Types.BaseGraph;

namespace CPP_Metrics.Types
{
    public class CBOEdge : IEdge<CBOVertex>
    {
        public CBOVertex From { get; set; }
        public CBOVertex To { get; set; }
        public decimal Price { get; set; }
    }
}
