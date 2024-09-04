using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services.AnalystService;

public class AnalystService : IAnalystService
{
    private readonly IServiceProvider _serviceProvider;

    public AnalystService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private DataGroup JoinTheEdgeTable(IQueryable<DataGroup> validDataSetTable, GraphQueryInfoDto graphQueryInfoDto)
    {
        var edgeTime = Stopwatch.StartNew();
        var edges = validDataSetTable.Include(s => s.VertexEntity)
            .ThenInclude(e => e.VertexAttributes.Where(att => att.Name == graphQueryInfoDto.vertexIdentifier))
            .ThenInclude(att => att.VertexValues).FirstOrDefault();
        edgeTime.Stop();
        Console.WriteLine("it take (" + edgeTime.ElapsedMilliseconds + ") to join th edge table");
        return edges;
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

    private DataGroup JoinTheVertexTable(IQueryable<DataGroup> validDataSetTable)
    {
        var vertexTime = Stopwatch.StartNew();
        var vertexes = validDataSetTable.Include(s => s.EdgeEntity)
            .ThenInclude(e => e.EdgeAttributes)
            .ThenInclude(att => att.EdgeValues).FirstOrDefault();
        vertexTime.Stop();
        Console.WriteLine("it take (" + vertexTime.ElapsedMilliseconds + ") to join th vertex table");
        return vertexes;
    }

    public async Task<ServiceResponse<DisplayGraphDto>> GetTheVertexNeighbor(GraphQueryInfoDto graphQueryInfoDto,
        string vertexId)
    {
        Console.WriteLine("%%%%%%%&&&&&&&-------------start to Getting the Vertex Neigbor-----------%%%%%%%&&&&&&&\n");
        var time = Stopwatch.StartNew();
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var validDataSetTable = context.DataSets.Where(ds => ds.DataGroupId == graphQueryInfoDto.datasetId);
        var vertexes = JoinTheVertexTable(validDataSetTable);
        var edges = JoinTheEdgeTable(validDataSetTable, graphQueryInfoDto);
        var BaseValue = vertexes.VertexEntity.VertexAttributes
            .FirstOrDefault(att => att.Name == graphQueryInfoDto.vertexIdentifier)!
            .VertexValues.FirstOrDefault(value => value.ObjectId == vertexId);
        Console.WriteLine("value of the target vertex has been found and its: " + BaseValue.StringValue + "\n");
        if (BaseValue is null)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.BadRequest,
                "invalid input for vertex field");

        if (vertexes is null)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.BadRequest, "database Error");
        var find = findTargetAtt(vertexes, graphQueryInfoDto);
        var targetAtt = find.targetAtt;
        var sourcesAtt = find.sourcesAtt;

        if (targetAtt is null || sourcesAtt is null)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.BadRequest,
                "invalid input for the edge field");

        var validTargetValueList = targetAtt.EdgeValues.Where(val => val.StringValue == BaseValue.StringValue);
        var validSourseValueList = sourcesAtt.EdgeValues.Where(val => val.StringValue == BaseValue.StringValue);

        var validEdgeTarget = validTargetValueList.Select(v => new Edge()
        {
            Id = v.ObjectId,
            Source = edges.EdgeEntity.EdgeAttributes.FirstOrDefault(att => att.Name == sourcesAtt.Name).EdgeValues
                .FirstOrDefault(val => val.ObjectId == v.ObjectId).StringValue,
            Target = v.StringValue
        });
        var validEdgeSource = validSourseValueList.Select(v => new Edge()
        {
            Id = v.ObjectId,
            Target = edges.EdgeEntity.EdgeAttributes.FirstOrDefault(att => att.Name == targetAtt.Name).EdgeValues
                .FirstOrDefault(val => val.ObjectId == v.ObjectId).StringValue,
            Source = v.StringValue
        });
        var validEdge = validEdgeTarget.Union(validEdgeSource);
        HashSet<string> validVertexId = new();
        List<string> validVertexLabel = new();
        List<Edge> validEdges = new();
        var vertexValueList =
            new List<VertexValue>(context.VertexAttributes.Find(BaseValue.VertexAttributeId)?.VertexValues);
        Console.WriteLine("start a big for ........");
        var forTime = Stopwatch.StartNew();
        int i = 0, j = 0, k = 0;
        foreach (var edgeValue in validEdge)
        {
            i++;
            foreach (var source in vertexValueList)
            {
                j++;
                if (edgeValue.Source == source.StringValue)
                    foreach (var target in vertexValueList)
                    {
                        k++;
                        if (edgeValue.Target == target.StringValue)
                        {
                            validVertexId.Add(source.ObjectId);
                            validVertexId.Add(target.ObjectId);
                            validVertexLabel.Add(source.StringValue);
                            validVertexLabel.Add(target.StringValue);
                            validEdges.Add(new Edge()
                                { Id = edgeValue.Id, Source = source.ObjectId, Target = target.ObjectId });
                        }
                    }
            }
        }

        forTime.Stop();
        Console.WriteLine("finish the huge for and total prosece is : (" + i * j * k + " )");
        Console.WriteLine("time that take that to finish the for : " + forTime.ElapsedMilliseconds + "");

        var idList = new List<string>(validVertexId);
        if (validVertexId.Count != validVertexLabel.Count)
            return new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.InternalServerError,
                "dont have sink output");

        List<Vertex> resultVertex = new();
        for (var l = 0; l < validVertexId.Count; l++)
            resultVertex.Add(new Vertex() { Id = idList[l], Label = validVertexLabel[l] });

        Console.WriteLine("finish the creating of vertex");
        var responseData = new DisplayGraphDto()
            { GraphId = graphQueryInfoDto.datasetId, Edges = validEdges, Vertices = resultVertex };
        time.Stop();
        Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@  that take (" + time.ElapsedMilliseconds +
                          ") milieSec to find the VertexNeighbor @@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        return new ServiceResponse<DisplayGraphDto>(responseData, ApiResponseType.Success, string.Empty);
    }
}