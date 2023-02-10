namespace CPP_Metrics.Types.Context
{
    public interface INestedName
    {
        public string Name { get; set; }
    }
    public class SimpleNestedName : INestedName
    {
        public SimpleNestedName(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
    public class TemplateNestedName : INestedName
    {
        public string Name { get; set; }
        public IList<string>? Templates { get; set; }

        public TemplateNestedName(string name)
        {
            Name = name;
        }
    }


}
