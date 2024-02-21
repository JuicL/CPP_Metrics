using CPP_Metrics.Visitors.OOP;

namespace CPP_Metrics.Types.Context
{
    public class BasedClassInfo : CPPType
    {
        public bool VirtualMarker { get; set; } = false;
        public AccesSpecifier AccesSpecifier { get; set; }
    }

}
