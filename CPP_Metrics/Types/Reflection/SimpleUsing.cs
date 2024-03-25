namespace CPP_Metrics.Types.Reflection
{
    public class SimpleUsing
    {
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new List<CPPType>();
        public ClassStructDeclaration? BaseContextElement { get; set; }
    }
}
