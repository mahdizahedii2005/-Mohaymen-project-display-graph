using System.Linq;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class GraphService : IGraphService
{
    private readonly IVertexService _vertexService;
    private readonly IEdgeService _edgeService;

    public GraphService(IVertexService vertexService, IEdgeService edgeService)
    {
        _vertexService = vertexService;
        _edgeService = edgeService;
    }

    private void GetSourcesAndDestinations(Dictionary<string, List<Vertex>> vertxAttValues,
        string sourceValue, string destinationValue, out List<Vertex> sources, out List<Vertex> destinations)
    {
        //sources = new List<Vertex>();
        //destinations = new List<Vertex>();

        sources = vertxAttValues[sourceValue];
        destinations = vertxAttValues[destinationValue];
        /*
        foreach (var v in vertices)
        {
            if (v.Label == sourceValue)
            {
                sources.Add(v);
            }
            if (v.Label == destinationValue)
            {
                destinations.Add(v);
            }
        }*/

        /*
        foreach (var record1 in vertexRecords)
        {
            foreach (var item in record1)
            {
                if (item.VertexAttribute.Name == vertexIdentifierFieldName && item.StringValue == sourceValue)
                {
                    Vertex vertex = new Vertex()
                    {
                        Id = record1.Key
                    };
                    sources.Add(vertex);
                }

                if (item.VertexAttribute.Name == vertexIdentifierFieldName && item.StringValue == destinationValue)
                {
                    Vertex vertex = new Vertex()
                    {
                        Id = record1.Key
                    };
                    destinations.Add(vertex);
                }
            }
        }
        */
    }

    private void GetSourceAndDerstinationValues(string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, IGrouping<string, EdgeValue> record, out string sourceValue,
        out string destinationValue)
    {
        sourceValue = string.Empty;
        destinationValue = string.Empty;
        foreach (var item in record)
        {
            if (item.EdgeAttribute.Name == sourceEdgeIdentifierFieldName) sourceValue = item.StringValue;

            if (item.EdgeAttribute.Name == destinationEdgeIdentifierFieldName) destinationValue = item.StringValue;
        }
    }


    public (List<Vertex> vertices, List<Edge> edges) GetGraph(Dictionary<string, Dictionary<string, string>> vertices,
        Dictionary<string, Dictionary<string, string>> edges, string vertexIdentifierFieldName,
        string SourceIdentifierFieldName, string TargetIdentifierFieldName)
    {
        var resEdges = new List<Edge>();

        var dicVertices = vertices.GroupBy(x => x.Value[vertexIdentifierFieldName])
            .ToDictionary(x => x.Key, x => x.ToList());

        var resVertices = vertices
            .Select(record => new Vertex
            {
                Id = record.Key,
                Label = record.Value[vertexIdentifierFieldName]
            })
            .ToList();

        foreach (var edge in edges)
        {
            var sourceValue = edge.Value[SourceIdentifierFieldName];
            var targetValue = edge.Value[TargetIdentifierFieldName];

            List<KeyValuePair<string, Dictionary<string, string>>> sources;
            if (!dicVertices.TryGetValue(sourceValue, out sources)) continue;
            List<KeyValuePair<string, Dictionary<string, string>>> targets;
            if (!dicVertices.TryGetValue(targetValue, out targets)) continue;

            foreach (var source in sources)
                foreach (var target in targets)
                {
                    var newEdge = new Edge()
                    {
                        Id = edge.Key,
                        Source = source.Key,
                        Target = target.Key
                    };
                    resEdges.Add(newEdge);
                }
        }

        return (resVertices, resEdges);
    }
}