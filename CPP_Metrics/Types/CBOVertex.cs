using CPP_Metrics.Types.BaseGraph;

namespace CPP_Metrics.Types
{
    public class CBOVertex : IVertex
    {
        public Guid Id { get; set; }
        // Полный путь до типа
        public string FullName { get; set; }
        public string? Namespace { get; set; }
        public bool IsProjectClass { get; set; } = false;
        public string FileName { get; set; } = "";
    }
}
