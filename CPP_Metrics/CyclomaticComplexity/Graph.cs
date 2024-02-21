using CPP_Metrics.Types.BaseGraph;

namespace CPP_Metrics.CyclomaticComplexity
{

    public class CyclomaticGraph: Graph<CyclomaticVertex,CyclomaticEdge>
    {
        public CyclomaticVertex Head { get; set; }
        public CyclomaticVertex Tail { get; set; }
        public CyclomaticGraph()
        {
            var head = this.CreateVertex();
            head.Type = Type.Head;
            Head = head;

            var tail = this.CreateVertex();
            tail.Type = Type.Tail;
            Tail = tail;
        }
    }
}
