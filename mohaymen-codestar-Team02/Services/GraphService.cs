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
            if (item.EdgeAttribute.Name == sourceEdgeIdentifierFieldName)
            {
                sourceValue = item.StringValue;
            }

            if (item.EdgeAttribute.Name == destinationEdgeIdentifierFieldName)
            {
                destinationValue = item.StringValue;
            }
        }
    }


    public (List<Vertex> vertices, List<Edge> edges) GetGraph(Dictionary<string, Dictionary<string, string>> vertices,
        Dictionary<string, Dictionary<string, string>> edges, string vertexIdentifierFieldName,
        string SourceIdentifierFieldName, string TargetIdentifierFieldName)
    {
        List<Edge> resEdges = new List<Edge>();

        var dicVertices = vertices.GroupBy(x => x.Value[vertexIdentifierFieldName])
            .ToDictionary(x => x.Key, x => x.ToList());

        /*
        List<Vertex> resVertices = new List<Vertex>();

        foreach (var record in vertices)
        {
            var newVertex = new Vertex()
            {
                Id = record.Key,
                Label = record.Value[vertexIdentifierFieldName]
            };
            resVertices.Add(newVertex);
        }*/
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
            if (!dicVertices.TryGetValue(sourceValue, out sources))
            {
                continue;
            }
            List<KeyValuePair<string, Dictionary<string, string>>> targets;
            if (!dicVertices.TryGetValue(targetValue, out targets))
            {
                continue;
            }

            // var sources = dicVertices[sourceValue];
            // var targets = dicVertices[targetValue];

            foreach (var source in sources)
            {
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
        }

        return (resVertices, resEdges);
    }

    /*
    foreach (var edge in edges)
    {
        var sources = groupedVertices[edge["StringValue"]];
        var destinations = groupedVertices[edge["StringValue"]];
        foreach (var source in sources)
        {
            foreach (var des in destinations)
            {
                /*
                Edge newEdge = new Edge()
                {
                    Id = record.Key,
                    Source = source.Id,
                    Target = des.Id
                };
                edge.Source = source.Id;
                edge.Target = des.Id;
                res.Add(edge);
            }
        }
        
    }*/
           /* 
        foreach (var edge in edges)
        {
            var sources = dicList[edge.Source];
            var destinations = dicList[edge.Target];


            //return sources.SelectMany(source => destinations.Select(des => edge)).ToList();

            foreach (var source in sources)
            {
                foreach (var des in destinations)
                {
                    /*
                    Edge newEdge = new Edge()
                    {
                        Id = record.Key,
                        Source = source.Id,
                        Target = des.Id
                    };
                    edge.Source = source.Id;
                    edge.Target = des.Id;
                    res.Add(edge);
                }
            }*/
            /*
            foreach (var record in edgeRecords)
            {
                GetSourceAndDerstinationValues(sourceEdgeIdentifierFieldName, destinationEdgeIdentifierFieldName, record,
                    out var sourceValue, out var destinationValue);
    
                GetSourcesAndDestinations(dicList, sourceValue, destinationValue,
                    out var sources, out var destinations);
    
                foreach (var source in sources)
                {
                    foreach (var des in destinations)
                    {
                        Edge edge = new Edge()
                        {
                            Id = record.Key,
                            Source = source.Id,
                            Target = des.Id
                        };
                        edges.Add(edge);
                    }
                }*/


        /*
    public (List<Vertex> vertices, List<Edge> edges) GetGraph(long dataSetID, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName, Dictionary<string, string> vertexAttributeVales, Dictionary<string, string> edgeAttributeValues)
    {
        var vertices = _vertexService.GetAllVertices(dataSetID, vertexIdentifierFieldName, vertexAttributeVales);
        var edges = _edgeService.GetAllEdges(dataSetID, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName, edgeAttributeValues);
        
        
        
        return (vertices, edges);
    }*/
}
