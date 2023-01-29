
namespace CPP_Metrics.Types
{

    
    /// <summary>
    /// Обобщенный граф
    /// </summary>
    /// <typeparam name="TVertex">Тип вершины</typeparam>
    /// <typeparam name="TEdge">Тип ребра</typeparam>
    public class Graph<TVertex, TEdge>
        where TVertex : class, IVertex, new()
        where TEdge : IEdge<TVertex>, new()
    {
        public ISet<TVertex> Verticies { get; private set; }
        public ISet<TEdge> Edges { get; private set; }

        public Graph()
        {
            Verticies = new HashSet<TVertex>();
            Edges = new HashSet<TEdge>();
        }

        /// <summary>
        /// Создание вершины
        /// </summary>
        /// <returns></returns>
        public TVertex CreateVertex()
        {
            var vertex = new TVertex { Id = System.Guid.NewGuid() };
            Verticies.Add(vertex);
            return vertex;
        }

        /// <summary>
        /// Создание ребра
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public TEdge CreateEdge(TVertex from, TVertex to)
        {
            var edge = new TEdge
            {
                From = from,
                To = to
            };
            Edges.Add(edge);
            return edge;
        }

        /// <summary>
        /// Очистка графа
        /// </summary>
        public void Clear()
        {
            Edges.Clear();
            Verticies.Clear();
        }

        /// <summary>
        /// Список смежных с вершиной вершин
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public ISet<TVertex> this[TVertex vertex]
        {
            get => Edges
                .Where(x => x.From == vertex)
                .Select(x => x.To)
                .Distinct()
                .ToHashSet();
        }

    }
}
