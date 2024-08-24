using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public class VertexService : IVertexService
{
    private IServiceProvider _serviceProvider;

    public VertexService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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