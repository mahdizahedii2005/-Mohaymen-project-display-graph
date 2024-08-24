using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class EdgeService : IEdgeService
{
    private IServiceProvider _serviceProvider;

    public EdgeService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public List<Edge<string>> GetAllEdges(string datasetName)
    {
        throw new NotImplementedException();
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