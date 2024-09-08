using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Exception;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;

namespace mohaymen_codestar_Team02.Services.AnalystService;

public class AnalystService : IAnalystService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IVertexService _vertexService;
    private readonly IEdgeService _edgeService;
    private readonly IGraphService _graphService;

    public AnalystService(IServiceProvider serviceProvider, IVertexService vertexService, IEdgeService edgeService,
        IGraphService graphService)
    {
        _serviceProvider = serviceProvider;
        _vertexService = vertexService;
        _edgeService = edgeService;
        _graphService = graphService;
    }

    // Method broken into smaller parts
    public async Task<ServiceResponse<DisplayGraphDto>> GetTheVertexNeighbor(GraphQueryInfoDto graphQueryInfoDto,
        string vertexId)
    {
        Console.WriteLine("%%%%%%%&&&&&&&-------------start to Getting the Vertex Neighbor-----------%%%%%%%&&&&&&&\n");

        // 1. ایجاد کانتکست و چک کردن دیتابیس
        var context = GetDbContext();
        if (context == null) throw new DatabaseExceptionCantFindDataBase();

        // 2. گرفتن اطلاعات مربوط به vertex و edge
        var vertexes = GetVertexes(context, graphQueryInfoDto);
        var edges = GetEdges(context, graphQueryInfoDto);
        if (vertexes is null || edges is null) throw new DatabaseExceptionCantFindDataBase();

        // 3. پیدا کردن مقدار BaseValue برای vertex هدف
        var baseValue = FindBaseValue(vertexes, graphQueryInfoDto, vertexId);
        if (baseValue == null)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.BadRequest, "invalid input for vertex field");

        // 4. پیدا کردن Attributes مرتبط با source و target
        var (sourcesAtt, targetAtt) = FindTargetAttributes(vertexes, graphQueryInfoDto);
        if (targetAtt == null || sourcesAtt == null)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.BadRequest, "invalid input for the edge field");

        // 5. محاسبه لبه‌های معتبر
        var validEdges = GetValidEdges(edges, baseValue, sourcesAtt, targetAtt);

        // 6. پردازش داده‌ها و یافتن Vertexهای معتبر
        var (validVertexId, validVertexLabel, validEdgeList) = ProcessValidEdges(context, vertexes, validEdges);

        // 7. ساخت و بازگشت نتیجه
        return CreateResponse(graphQueryInfoDto, validVertexId, validVertexLabel, validEdgeList);
    }

    private DataContext GetDbContext()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DataContext>();
    }

    private DataGroup GetVertexes(DataContext context, GraphQueryInfoDto graphQueryInfoDto)
    {
        var validDataSetTable = context.DataSets.Where(ds => ds.DataGroupId == graphQueryInfoDto.datasetId);
        return JoinTheVertexTable(validDataSetTable);
    }

    private DataGroup GetEdges(DataContext context, GraphQueryInfoDto graphQueryInfoDto)
    {
        var validDataSetTable = context.DataSets.Where(ds => ds.DataGroupId == graphQueryInfoDto.datasetId);
        return JoinTheEdgeTable(validDataSetTable, graphQueryInfoDto);
    }

    private VertexValue FindBaseValue(DataGroup vertexes, GraphQueryInfoDto graphQueryInfoDto, string vertexId)
    {
        return vertexes.VertexEntity.VertexAttributes
            .FirstOrDefault(att => att.Name == graphQueryInfoDto.vertexIdentifier)?
            .VertexValues.FirstOrDefault(value => value.ObjectId == vertexId);
    }

    private (EdgeAttribute, EdgeAttribute) FindTargetAttributes(DataGroup vertexes, GraphQueryInfoDto graphQueryInfoDto)
    {
        return findTargetAtt(vertexes, graphQueryInfoDto);
    }

    private IEnumerable<Edge> GetValidEdges(DataGroup edges, VertexValue baseValue, EdgeAttribute sourcesAtt, EdgeAttribute targetAtt)
    {
        var validTargetValueList = targetAtt.EdgeValues.Where(val => val.StringValue == baseValue.StringValue);
        var validSourceValueList = sourcesAtt.EdgeValues.Where(val => val.StringValue == baseValue.StringValue);

        var validEdgeTarget = validTargetValueList.Select(v => new Edge()
        {
            Id = v.ObjectId,
            Source = edges.EdgeEntity.EdgeAttributes.FirstOrDefault(att => att.Name == sourcesAtt.Name).EdgeValues
                .FirstOrDefault(val => val.ObjectId == v.ObjectId).StringValue,
            Target = v.StringValue
        });

        var validEdgeSource = validSourceValueList.Select(v => new Edge()
        {
            Id = v.ObjectId,
            Target = edges.EdgeEntity.EdgeAttributes.FirstOrDefault(att => att.Name == targetAtt.Name).EdgeValues
                .FirstOrDefault(val => val.ObjectId == v.ObjectId).StringValue,
            Source = v.StringValue
        });

        return validEdgeTarget.Union(validEdgeSource);
    }

    private (HashSet<string>, List<string>, List<Edge>) ProcessValidEdges(DataContext context, DataGroup vertexes, IEnumerable<Edge> validEdge)
    {
        HashSet<string> validVertexId = new();
        List<string> validVertexLabel = new();
        List<Edge> validEdges = new();
        var vertexValueList = new List<VertexValue>(context.VertexAttributes.Find(vertexes.VertexEntity.VertexAttributes.First().Id)?.VertexValues);

        foreach (var edgeValue in validEdge)
        {
            foreach (var source in vertexValueList)
            {
                if (edgeValue.Source == source.StringValue)
                {
                    foreach (var target in vertexValueList)
                    {
                        if (edgeValue.Target == target.StringValue)
                        {
                            validVertexId.Add(source.ObjectId);
                            validVertexId.Add(target.ObjectId);
                            validVertexLabel.Add(source.StringValue);
                            validVertexLabel.Add(target.StringValue);
                            validEdges.Add(new Edge()
                            {
                                Id = edgeValue.Id,
                                Source = source.ObjectId,
                                Target = target.ObjectId
                            });
                        }
                    }
                }
            }
        }

        return (validVertexId, validVertexLabel, validEdges);
    }

    private ServiceResponse<DisplayGraphDto> CreateResponse(GraphQueryInfoDto graphQueryInfoDto, HashSet<string> validVertexId, List<string> validVertexLabel, List<Edge> validEdges)
    {
        var idList = validVertexId.ToList();
        if (validVertexId.Count != validVertexLabel.Count)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.InternalServerError, "don't have sink output");

        List<Vertex> resultVertex = new();
        for (var l = 0; l < validVertexId.Count; l++)
        {
            resultVertex.Add(new Vertex() { Id = idList[l], Label = validVertexLabel[l] });
        }

        var responseData = new DisplayGraphDto()
        {
            GraphId = graphQueryInfoDto.datasetId,
            Edges = validEdges,
            Vertices = resultVertex
        };

        return new ServiceResponse<DisplayGraphDto>(responseData, ApiResponseType.Success, string.Empty);
    }

    private DataGroup JoinTheEdgeTable(IQueryable<DataGroup> validDataSetTable, GraphQueryInfoDto graphQueryInfoDto)
    {
        var edgeTime = Stopwatch.StartNew();
        var edges = validDataSetTable.Include(s => s.VertexEntity)
            .ThenInclude(e => e.VertexAttributes.Where(att => att.Name == graphQueryInfoDto.vertexIdentifier))
            .ThenInclude(att => att.VertexValues).FirstOrDefault();
        edgeTime.Stop();
        Console.WriteLine("it takes (" + edgeTime.ElapsedMilliseconds + ") ms to join the edge table");
        return edges;
    }

    private DataGroup JoinTheVertexTable(IQueryable<DataGroup> validDataSetTable)
    {
        var vertexTime = Stopwatch.StartNew();
        var vertexes = validDataSetTable.Include(s => s.EdgeEntity)
            .ThenInclude(e => e.EdgeAttributes)
            .ThenInclude(att => att.EdgeValues).FirstOrDefault();
        vertexTime.Stop();
        Console.WriteLine("it takes (" + vertexTime.ElapsedMilliseconds + ") ms to join the vertex table");
        return vertexes;
    }

    private (EdgeAttribute sourcesAtt, EdgeAttribute targetAtt) findTargetAtt(DataGroup vertexes,
        GraphQueryInfoDto graphQueryInfoDto)
    {
        EdgeAttribute sourcesAtt = null;
        EdgeAttribute targetAtt = null;
        var edgeAttList = new List<EdgeAttribute>(vertexes.EdgeEntity.EdgeAttributes);
        foreach (var att in edgeAttList)
        {
            if (att.Name.ToLower() == graphQueryInfoDto.sourceIdentifier.ToLower())
                sourcesAtt = att;
            else if (att.Name.ToLower() == graphQueryInfoDto.targetIdentifier.ToLower()) targetAtt = att;

            if (targetAtt is not null && sourcesAtt is not null) return (sourcesAtt, targetAtt);
        }

        return (sourcesAtt, targetAtt);
    }

    public async Task<ServiceResponse<DisplayGraphDto>> DisplayGeraphData(long databaseId,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName,
        Dictionary<string, string> vertexAttributeValus, Dictionary<string, string> edgeAttributeValues)
    {
        var vertices = _vertexService.GetAllVertices(databaseId, vertexIdentifierFieldName, vertexAttributeValus);
        var edges = _edgeService.GetAllEdges(databaseId, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName, edgeAttributeValues);
        var graph = _graphService.GetGraph(vertices, edges, vertexIdentifierFieldName, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName);

        var dto = new DisplayGraphDto()
        {
            Vertices = graph.vertices,
            Edges = graph.edges
        };
        return new ServiceResponse<DisplayGraphDto>(dto, ApiResponseType.Success,
            Resources.GraphFetchedSuccessfullyMessage);
    }

    public ServiceResponse<List<GetAttributeDto>> GetVertexAttributes(long vertexEntityId)
    {
        var att = _vertexService.GetVertexAttributes(vertexEntityId);
        return new ServiceResponse<List<GetAttributeDto>>(att, ApiResponseType.Success, "");
    }

    public ServiceResponse<List<GetAttributeDto>> GetEdgeAttributes(long edgeEntityId)
    {
        var att = _edgeService.GetEdgeAttributes(edgeEntityId);
        return new ServiceResponse<List<GetAttributeDto>>(att, ApiResponseType.Success, "");
    }
}
