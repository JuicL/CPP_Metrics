
namespace CPP_Metrics.Types
{
    public class Identifier
    {
        public string? Name { get; set; }
    }
    public class TemplateIdentifier : Identifier
    {
        public IList<Identifier>? Templates;
    }

}
