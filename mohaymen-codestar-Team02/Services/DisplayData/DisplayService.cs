using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.DynamicService;
using mohaymen_codestar_Team02.Services.TokenService;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class DisplayService : IDisplayDataService
{
    private readonly DataContext _context;

    public DisplayService(DataContext context)
    {
        _context = context;
    }

    public (List<Vertex> vertices, List<Edge> edges) GetGraph(string databaseName, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName)
    {
        var dataSet = _context.DataSets.Include(ds => ds.VertexEntity)
            .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv=>vv.VertexValues).Include(ds => ds.EdgeEntity)
            .ThenInclude(ee => ee.EdgeAttributes).ThenInclude(ev=>ev.EdgeValues).FirstOrDefault(ds=>ds.Name.ToLower().Equals(databaseName.ToLower()));

        var vertexRecords = dataSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        List<Vertex> vertices = new List<Vertex>();
        foreach (var record in vertexRecords)
        {
            var value = record.SingleOrDefault(r => r.VertexAttribute.Name == vertexIdentifierFieldName).StringValue;
            Vertex v = new Vertex()
            {
                Id = record.Key,
                Value = value
            };
            vertices.Add(v);
        }

        var edgeRecords = dataSet.EdgeEntity.EdgeAttributes.Select(ea => ea.EdgeValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        List<Edge> edges = new List<Edge>();
        foreach (var record in edgeRecords)
        {
            string sourceValue = string.Empty;
            string destinationValue = string.Empty;
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
            
            List<Vertex> sources = new List<Vertex>();
            List<Vertex> destinations = new List<Vertex>();
            
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
            }
        }

        return (vertices, edges);
    }
    
    /*
    public void GetGraph(string databaseName, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName, bool directed,
        out List<dynamic> vertices, List<dynamic> edges)
    {
        // get dataset
        var dataSet = _context.DataSets.Include(ds => ds.VertexEntity)
            .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv => vv.VertexValues).Include(ds => ds.EdgeEntity)
            .ThenInclude(ee => ee.EdgeAttributes).ThenInclude(ev => ev.EdgeValues)
            .FirstOrDefault(ds => ds.Name.ToLower().Equals(databaseName.ToLower()));

        // get vertex info
        var vertexTypeName = dataSet.VertexEntity.Name;

        var vertexFieldNames = dataSet.VertexEntity.VertexAttributes.Select(a => a.Name).ToList();

        var vertexRecords = dataSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId)
            .Select(g => g.ToDictionary(v => v.VertexAttribute.Name, v => v.StringValue)).ToList();

        var vertexFieldNamesTypes = new Dictionary<string, Type>();
        foreach (var vertexFieldNameType in vertexFieldNames)
        {
            vertexFieldNamesTypes.Add(vertexFieldNameType, typeof(string));
        }

        var vertexType = _modelBuilder.CreateDynamicClass(vertexTypeName, vertexFieldNamesTypes, null);

        vertices = new List<dynamic>(); //
        foreach (var vertexRecord in vertexRecords)
        {
            vertices.Add(_objectBuilder.CreateDynamicObject(vertexType, vertexRecord));
        }


        // get edge info
        var edgeTypeName = dataSet.EdgeEntity.Name;
        var EdgeFieldNames = dataSet.EdgeEntity.EdgeAttributes.Select(a => a.Name).ToList();
        var edgeRecords = dataSet.EdgeEntity.EdgeAttributes.Select(ea => ea.EdgeValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId)
            .Select(g => g.ToDictionary(v => v.EdgeAttribute.Name, v => v.StringValue)).ToList();

        // delete from field names
        var edgeFieldNameTypes = new Dictionary<string, Type>(); //

        edgeFieldNameTypes.Add("Source", vertexType);
        edgeFieldNameTypes.Add("Target", vertexType);

        foreach (var edgeFieldNameType in EdgeFieldNames)
        {
            edgeFieldNameTypes.Add(edgeFieldNameType, typeof(string));
        }

        var vertexType1 = typeof(IEdge<>).MakeGenericType(vertexType);
        var edgeType = _modelBuilder.CreateDynamicClass(edgeTypeName, edgeFieldNameTypes, vertexType1);

        edges = new List<dynamic>(); //
        // get valid edgges
        List<Dictionary<string, string>> sources = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> destinations = new List<Dictionary<string, string>>();

        foreach (var er in edgeRecords)
        {
            foreach (var vr in vertexRecords)
            {
                if (vr[vertexIdentifierFieldName].Equals(er[sourceEdgeIdentifierFieldName]))
                {
                    sources.Add(vr);
                }

                if (vr[vertexIdentifierFieldName].Equals(er[destinationEdgeIdentifierFieldName]))
                {
                    destinations.Add(vr);
                }
            }

            if (sources.Count != 0 || destinations.Count != 0)
            {
                // error
            }
            else
            {
                // create edges
                foreach (var source in sources)
                {
                    foreach (var destination in destinations)
                    {
                        edges.Add(_objectBuilder.CreateDynamicObject1(edgeType, er, vertexType, source, destination));
                    }
                }
            }
        }
    }*/
}