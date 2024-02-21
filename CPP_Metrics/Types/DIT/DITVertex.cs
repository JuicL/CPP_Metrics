

using CPP_Metrics.Types.BaseGraph;

namespace CPP_Metrics.Types.DIT
{
    public class DITVertex : IVertex
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ParenCount { get; set; } = 0;

    }
}
