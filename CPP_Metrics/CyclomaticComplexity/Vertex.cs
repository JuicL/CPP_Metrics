

using CPP_Metrics.Types;

namespace CPP_Metrics.CyclomaticComplexity
{
    public enum Type
    {
        Head,
        Tail,
        EmpyStatement,
        Statment,
        If,
        Else,
        While,
        For,
        Do,
        Switch,
        Jump,
        Case
    }
    public class CyclomaticVertex : IVertex
    {
        public CyclomaticVertex() { }
        public CyclomaticVertex(Type type)
        {
            Type = type;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = "";

        public double Value { get; set; } = 0;
        public Type Type { get; set; }

    }
    public class Vertex : IVertex
    {
        public Vertex()
        { 
        }

        public Vertex( Type type)
        {
            Type = type;
        }
        public string Name { get; set; } = "";

        public double Value { get; set; } = 0;
        public Type Type { get; set; }

        public IList<Vertex> Children { get; set; } = new List<Vertex>();
        Guid IVertex.Id { get; set; }
    }
}
