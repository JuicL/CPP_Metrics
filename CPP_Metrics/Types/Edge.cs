
namespace CPP_Metrics.Types
{
    /// <summary>
    /// Обобщенное ребро
    /// </summary>
    /// <typeparam name="TVertex">Тип веришины</typeparam>
    public interface IEdge<TVertex>
        where TVertex : class, IVertex
    {
        TVertex From { get; set; }
        TVertex To { get; set; }
        decimal Price { get; set; }
    }
}
