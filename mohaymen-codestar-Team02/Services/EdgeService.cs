using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class EdgeService : IEdgeService
{
    private readonly IServiceProvider _serviceProvider;

    public EdgeService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public List<Edge> GetAllEdges(string databaseName, string vertexIdentifierFieldName,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName)
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var dataSet = context.DataSets.Include(ds => ds.VertexEntity)
            .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv => vv.VertexValues).Include(ds => ds.EdgeEntity)
            .ThenInclude(ee => ee.EdgeAttributes).ThenInclude(ev => ev.EdgeValues)
            .FirstOrDefault(ds => ds.Name.ToLower().Equals(databaseName.ToLower()));

        var vertexRecords = dataSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        var edgeRecords = dataSet.EdgeEntity.EdgeAttributes.Select(ea => ea.EdgeValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        List<Edge> edges = new();
        foreach (var record in edgeRecords)
        {
            GetSourceAndDerstinationValues(sourceEdgeIdentifierFieldName, destinationEdgeIdentifierFieldName, record,
                out var sourceValue, out var destinationValue);

            GetSourcesAndDestinations(vertexIdentifierFieldName, vertexRecords, sourceValue, destinationValue,
                out var sources, out var destinations);

            foreach (var source in sources)
                foreach (var des in destinations)
                {
                    var edge = new Edge()
                    {
                        Id = record.Key,
                        Source = source.Id,
                        Target = des.Id
                    };
                    edges.Add(edge);
                }
        }

        return edges;
    }

    private void GetSourcesAndDestinations(string vertexIdentifierFieldName,
        IEnumerable<IGrouping<string, VertexValue>> vertexRecords,
        string sourceValue, string destinationValue, out List<Vertex> sources, out List<Vertex> destinations)
    {
        sources = new List<Vertex>();
        destinations = new List<Vertex>();

        foreach (var record1 in vertexRecords)
            foreach (var item in record1)
            {
                if (item.VertexAttribute.Name == vertexIdentifierFieldName && item.StringValue == sourceValue)
                {
                    var vertex = new Vertex()
                    {
                        Id = record1.Key
                    };
                    sources.Add(vertex);
                }

                if (item.VertexAttribute.Name == vertexIdentifierFieldName && item.StringValue == destinationValue)
                {
                    var vertex = new Vertex()
                    {
                        Id = record1.Key
                    };
                    destinations.Add(vertex);
                }
            }
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

    public DetailDto GetEdgeDetails(string objId)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var validValue = context.EdgeValues.Where(value => value.ObjectId.ToLower() == objId.ToLower()).ToList();
        var result = new DetailDto();
        foreach (var value in validValue)
            result.AttributeValue[context.EdgeAttributes.Find(value.EdgeAttributeId).Name] = value.StringValue;
        return result;
    }
}