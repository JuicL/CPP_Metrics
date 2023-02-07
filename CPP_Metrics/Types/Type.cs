
namespace CPP_Metrics.Types
{
    public interface IType
    {
        public string TypeName { get; set; }
    }
    public class SimpleType: IType
    {
        public string TypeName { get; set; }
    }

    public class UserType: IType
    {
        public string? ClassStructMarker { get; set; }
        public string TypeName { get; set; }
        public IList<INestedName>? NestedNames { get; set; }
    }
    public class UserTemplateType: UserType
    {
        public IList<IType> TemplateNames { get; set; }
    }
}
