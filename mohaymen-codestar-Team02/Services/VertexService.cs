using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.VertexEAV;

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

    public Dictionary<string, Dictionary<string, string>> GetAllVertices(long dataSetId, string vertexIdentifierFieldName, Dictionary<string, string> vertexAttributeVales)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var dataSet = context.DataSets.Where(ds => ds.DataGroupId == dataSetId).Include(ds => ds.VertexEntity)
            .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv => vv.VertexValues)
            .FirstOrDefault();

        var vertexRecords = dataSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        /*
        var validVertexRecords = new List<IGrouping<string, VertexValue>>();
        foreach (var record in vertexRecords)
        {
            var dic = record.ToDictionary(x => x.VertexAttribute.Name, x => x.StringValue);
            if (vertexAttributeVales.Where(x => x.Value == dic[x.Key]).Count()==vertexAttributeVales.Count)
            {
                validVertexRecords.Add(record);
            }    
        }*/
        
        var validVertexRecords = vertexRecords
            .Where(group =>
                vertexAttributeVales.All(attr =>
                    group.Any(v => v.VertexAttribute.Name == attr.Key && v.StringValue == attr.Value)));

        var res = validVertexRecords.ToDictionary(x => x.Key,
            x => x.ToDictionary(g => g.VertexAttribute.Name, g => g.StringValue));

        
        //var dicList = vertices.GroupBy(x => x.Label).ToDictionary(x=>x.Key, x=>x.ToList());

        //var result = validVertexRecords.Select(x => x.ToDictionary(g => g.VertexAttribute.Name, g => g.StringValue)).ToList();
        
        /*
        var vertices = validVertexRecords
            .Select(group =>
            {
                var value = group.Where(v => v.VertexAttribute.Name == vertexIdentifierFieldName)
                    .Select(v => v.StringValue).FirstOrDefault();
                
                return new Vertex
                {
                    Id = group.Key,
                    Label = value
                };
            })
            .ToList();
*/
        return res;
        
        /*
        var vertices = new List<Vertex>();
        foreach (var record in validVertexRecords)
        {
            var value = record.SingleOrDefault(r => r.VertexAttribute.Name == vertexIdentifierFieldName).StringValue;
            Vertex v = new Vertex()
            {
                Id = record.Key,
                Label = value
            };
            vertices.Add(v);    
        }*/
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