namespace CPP_Metrics.Types.Context
{

    public class CPPType
    {
        public string? ClassStructMarker { get; set; }
        public string TypeName { get; set; }
        public List<CPPType> NestedNames { get; set; } = new List<CPPType>();
        public virtual IList<CPPType>? TemplateNames { get; set; }
        public string ClassMarker { get; set; }

        public bool IsTemplate { get { return TemplateNames is not null; } }
    }

}
