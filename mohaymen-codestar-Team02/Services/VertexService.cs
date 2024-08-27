using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public class VertexService : IVertexService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    public VertexService(IServiceProvider serviceProvider, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }

    public List<GetAttributeDto> GetVertexAttributes(long vertexEntityId)
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var vertexAttribuite = context.VertexEntities.Include(ve => ve.VertexAttributes)
            .FirstOrDefault(ve => ve.VertexEntityId == vertexEntityId)
            ?.VertexAttributes;

        return vertexAttribuite.Select(va => _mapper.Map<GetAttributeDto>(va)).ToList();
    }

    public List<Vertex> GetAllVertices(long dataSetId, string vertexIdentifierFieldName)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var dataSet = context.DataSets.Where(ds => ds.DataGroupId == dataSetId).Include(ds => ds.VertexEntity)
            .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv => vv.VertexValues)
            .FirstOrDefault(ds => ds != null);

        var vertexRecords = dataSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        List<Vertex> vertices = new List<Vertex>();
        foreach (var record in vertexRecords)
        {
            var value = record.SingleOrDefault(r => r.VertexAttribute.Name == vertexIdentifierFieldName).StringValue;
            Vertex v = new Vertex()
            {
                Id = record.Key,
                Label = value
            };
            vertices.Add(v);
        }

        return vertices;
    }

    public DetailDto GetVertexDetails(string objId)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var validValue = context.VertexValues.Where(value => value.ObjectId.ToLower() == objId.ToLower()).ToList();
        var result = new DetailDto();
        foreach (var value in validValue)
        {
            result.AttributeValue[context.VertexAttributes.Find(value.VertexAttributeId).Name] = value.StringValue;
        }

        return result;
    }

    public List<Vertex> GetAllVertices(string datasetName)
    {
        throw new NotImplementedException();
    }
}