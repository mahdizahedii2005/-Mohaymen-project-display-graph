using System.Linq;
using System.Xml.Schema;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class EdgeService : IEdgeService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    public EdgeService(IServiceProvider serviceProvider, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }
    
    public List<GetAttributeDto> GetEdgeAttributes(long edgeEntityId)
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var edgeAttribuite = context.EdgeEntities.Include(ve => ve.EdgeAttributes)
            .FirstOrDefault(ve => ve.EdgeEntityId == edgeEntityId)
            ?.EdgeAttributes;

        return edgeAttribuite.Select(va => _mapper.Map<GetAttributeDto>(va)).ToList();
    }

    public Dictionary<string, Dictionary<string, string>> GetAllEdges(long dataSetId,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, Dictionary<string, string> edgeAttributeVales)
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        //var vertexSet = context.DataSets.Where(ds => ds.DataGroupId == dataSetId).Include(ds => ds.VertexEntity)
          //  .ThenInclude(ve => ve.VertexAttributes).ThenInclude(vv => vv.VertexValues).FirstOrDefault(ds => ds != null);
        var edgeSet = context.DataSets.Where(ds => ds.DataGroupId == dataSetId).Include(ds => ds.EdgeEntity)
            .ThenInclude(ee => ee.EdgeAttributes).ThenInclude(ev => ev.EdgeValues).FirstOrDefault(ds => ds != null);;

        //var vertexRecords = vertexSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
          //  .GroupBy(v => v.ObjectId);

        var edgeRecords = edgeSet.EdgeEntity.EdgeAttributes.Select(ea => ea.EdgeValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId);

        var validEdgeRecords = edgeRecords
            .Where(group =>
                edgeAttributeVales.All(attr =>
                    group.Any(v => v.EdgeAttribute.Name == attr.Key && v.StringValue == attr.Value)));

        //var res = validEdgeRecords.Select(x => x.ToDictionary(g => g.EdgeAttribute.Name, g => g.StringValue)).ToList();
        
        var res = validEdgeRecords.ToDictionary(x => x.Key,
            x => x.ToDictionary(g => g.EdgeAttribute.Name, g => g.StringValue));        
        return res;
        
        var edges = validEdgeRecords
            .Select(group =>
            {
                var sourceValue = group.Where(v => v.EdgeAttribute.Name == sourceEdgeIdentifierFieldName)
                    .Select(v => v.StringValue).FirstOrDefault();
                
                var targeValue = group.Where(v => v.EdgeAttribute.Name == destinationEdgeIdentifierFieldName)
                    .Select(v => v.StringValue).FirstOrDefault();
                
                return new Edge()
                {
                    Id = group.Key,
                    Source = sourceValue,
                    Target = targeValue
                };
            })
            .ToList();

        /*
        var dic = record.ToDictionary(x => x.VertexAttribute.Name, x => x.StringValue);
        if (vertexAttributeVales.Where(x => x.Value == dic[x.Key]).Count()==vertexAttributeVales.Count)
        {
            break;
        }


        foreach (var rec in vertexRecords)
        {
            bool b = true;
            foreach (var vav in vertexAttributeVales)
            {
                foreach (var item in rec)
                {
                    if(vav.Value==item.StringValue)
                    
                }
            }
        }
        
        var vertexRecordsDic = vertexSet.VertexEntity.VertexAttributes.Select(a => a.VertexValues).SelectMany(v => v)
            .GroupBy(v => v.ObjectId).Select(g=>g.GetEnumerator().Current);

        var validVertexRecords = vertexRecordsDic.Where(dic =>
            vertexAttributeVales.Where(x => x.Value == dic[x.Key]).Count() == vertexAttributeVales.Count);
        */
        
        /*
        var validVertexRecords = new List<IGrouping<string, VertexValue>>();
        foreach (var record in vertexRecords)
        {
            var dic = record.ToDictionary(x => x.VertexAttribute.Name, x => x.StringValue);
            if (vertexAttributeVales.Where(x => x.Value == dic[x.Key]).Count()==vertexAttributeVales.Count)
            {
                validVertexRecords.Add(record);
            }    
        }
        */
        /*
        var validVertexRecords = vertexRecords
            .Where(record => 
                vertexAttributeVales.All(attributeValue => 
                    record.ToDictionary(x => x.VertexAttribute.Name, x => x.StringValue)
                        .ContainsKey(attributeValue.Key) && 
                    record.ToDictionary(x => x.VertexAttribute.Name, x => x.StringValue)[attributeValue.Key] == attributeValue.Value))
            .ToList();
        */

        /*
        var dicList = vertices.GroupBy(x => x.Label).ToDictionary(x=>x.Key, x=>x.ToList());
        
        List<Edge> edges = new List<Edge>();
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
            }
        }
*/
        //return edges;
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

    public DetailDto GetEdgeDetails(string objId)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var validValue = context.EdgeValues.Where(value => value.ObjectId.ToLower() == objId.ToLower()).ToList();
        var result = new DetailDto();
        foreach (var value in validValue)
        {
            result.AttributeValue[context.EdgeAttributes.Find(value.EdgeAttributeId).Name] = value.StringValue;
        }

        return result;
    }
}