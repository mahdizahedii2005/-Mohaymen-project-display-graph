using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;

namespace mohaymen_codestar_Team02.Services.AnalystService;

public class AnalystService : IAnalystService
{
    private readonly IServiceProvider _serviceProvider;

    public AnalystService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public DisplayGraphDto GetTheVertexNeighbor(GraphQueryInfoDto graphQueryInfoDto, string vertexId)
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var response = new DisplayGraphDto() { GraphId = graphQueryInfoDto.datasetId };
        // context.DataSets.Where(ds => ds.DataGroupId == graphQueryInfoDto.datasetId).Include()
        return response;
    }
}