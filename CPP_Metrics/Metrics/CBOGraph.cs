using CPP_Metrics.Types.BaseGraph;

namespace CPP_Metrics.Metrics
{
    public class CBOGraph: Graph<CBOVertex,CBOEdge>
    {
        public new CBOEdge? CreateEdge(CBOVertex v1,CBOVertex v2)
        {
            CBOEdge? edge = null;
            if(v1 != v2)
            {
                base.CreateEdge(v1,v2);
            }
            return edge;
        }
    }
}
