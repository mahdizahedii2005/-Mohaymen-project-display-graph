using mohaymen_codestar_Team02.Models.Graph.abstraction;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services.GraphBuilderService;

public interface IGraphHandler
{
    AdjacencyGraph<TVertex, TEdge> Creat<TEdge, TVertex>()
        where TVertex : Vertex where TEdge : Edge<TVertex>;

    AdjacencyGraph<TVertex, TEdge> Add<TEdge, TVertex>(AdjacencyGraph<TVertex, TEdge> graph, List<TVertex> vertices,
        List<TEdge> edges)
        where TVertex : Vertex where TEdge : Edge<TVertex>;
    AdjacencyGraph<TVertex, TEdge> Delete<TEdge, TVertex>(AdjacencyGraph<TVertex, TEdge> graph, List<TVertex> vertices,
        List<TEdge> edges)
        where TVertex : Vertex where TEdge : Edge<TVertex>;
}