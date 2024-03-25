using CPP_Metrics.Types.BaseGraph;

namespace CPP_Metrics.Metrics.CyclomaticComplexity
{
    public class CyclomaticEdge : IEdge<CyclomaticVertex>
    {
        public CyclomaticVertex From { get; set; }
        public CyclomaticVertex To { get; set; }
        public decimal Price { get; set; }
    }
    public class Edge
    {
        public Edge(Vertex from, Vertex to)
        {
            From = from;
            To = to;
        }

        public Vertex From { get; set; }
        public Vertex To { get; set; }
    }
}
