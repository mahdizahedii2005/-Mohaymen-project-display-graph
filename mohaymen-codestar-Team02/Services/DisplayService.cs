using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.TokenService;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class DisplayService
{

    private readonly DataContext _context;
    private readonly ModelBuilder _modelBuilder;
    private readonly ObjectBuilder _objectBuilder;

    public DisplayService(DataContext context, ModelBuilder modelBuilder, ObjectBuilder objectBuilder)
    {
        _context = context;
        _modelBuilder = modelBuilder;
        _objectBuilder = objectBuilder;
    }

    public void GetGraph(string databaseName, string sourceEdgeIdentifierFieldName, string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName, bool directed, out List<dynamic> vertices, List<dynamic> edges)
    {
        // get dataset
        var dataSet = _context.DataSets.Include(ds => ds.VertexEntity)
            .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv => vv.VertexValues).Include(ds => ds.EdgeEntity)
            .ThenInclude(ee => ee.EdgeAttributes).ThenInclude(ev => ev.EdgeValues).FirstOrDefault(ds => ds.Name.ToLower().Equals(databaseName.ToLower()));

        // get vertex info
        var vertexTypeName = dataSet.VertexEntity.Name;

        var vertexFieldNames = dataSet.VertexEntity.VertexAttributes.Select(a => a.Name).ToList();

        var vertexRecords = dataSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v).GroupBy(v => v.ObjectId)
            .Select(g => g.ToDictionary(v => v.VertexAttribute.Name, v => v.StringValue)).ToList();

        var vertexFieldNamesTypes = new Dictionary<string, Type>();
        foreach (var vertexFieldNameType in vertexFieldNamesTypes)
        {
            vertexFieldNamesTypes.Add(vertexFieldNameType.Key, typeof(string));
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
        var edgeRecords = dataSet.EdgeEntity.EdgeAttributes.Select(ea => ea.EdgeValues).SelectMany(v => v).GroupBy(v => v.ObjectId)
            .Select(g => g.ToDictionary(v => v.EdgeAttribute.Name, v => v.StringValue)).ToList();


        var edgeFieldNameTypes = new Dictionary<string, Type>(); //

        edgeFieldNameTypes.Add("Source", vertexType);
        edgeFieldNameTypes.Add("Target", vertexType);

        foreach (var edgeFieldNameType in edgeFieldNameTypes)
        {
            edgeFieldNameTypes.Add(edgeFieldNameType.Key, typeof(string));
        }

        var edgeType = _modelBuilder.CreateDynamicClass(edgeTypeName, edgeFieldNameTypes, typeof(IEdge<>));

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
                if (!directed)
                {
                    foreach (var source in sources)
                    {
                        foreach (var destination in destinations)
                        {
                            edges.Add(_objectBuilder.CreateDynamicObject1(edgeType, er, vertexType, destination, source));
                        }
                    }
                }

                foreach (var source in sources)
                {
                    foreach (var destination in destinations)
                    {
                        edges.Add(_objectBuilder.CreateDynamicObject1(edgeType, er, vertexType, source, destination));

                    }
                }
            }
        }
    }
}
