namespace CPP_Metrics.Types.Reflection
{
    public class UsingNamespace
    {
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new List<CPPType>();
    }
}
