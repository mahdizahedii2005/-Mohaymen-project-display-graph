using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService
    : IDataAdminService
{
    private readonly IStorHandler storHandler;
    private readonly IDisplayDataService _displayDataService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    public DataAdminService(IStorHandler storHandler, IDisplayDataService displayDataService, IMapper mapper, IServiceProvider serviceProvider)
    {
        this.storHandler = storHandler;
        _displayDataService = displayDataService;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    public async Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? edgeEntityName, string vertexEntityName, string userName)
    {
        try
        {
            if (string.IsNullOrEmpty(edgeEntityName) || string.IsNullOrEmpty(graphName) ||
                string.IsNullOrEmpty(vertexEntityName))
                return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest,
                    Data.Resources.InvalidInpute);

            var dataGroupId = await storHandler.StoreDataSet(graphName, userName);
            if (dataGroupId == -1)
                return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest,
                    Data.Resources.InvalidInpute);

            if (!await storHandler.EdageStorer.StoreFileData(edgeEntityName, edgeFile, dataGroupId))
                return new ServiceResponse<string>(string.Empty,
                    ApiResponseType.BadRequest, Data.Resources.InvalidInpute);

            if (!await storHandler.VertexStorer.StoreFileData(vertexEntityName, vertexFile, dataGroupId))
                return new ServiceResponse<string>(string.Empty,
                    ApiResponseType.BadRequest, Data.Resources.InvalidInpute);

            return new ServiceResponse<string>(null, ApiResponseType.Success, string.Empty);
        }
        catch (NullReferenceException e)
        {
            return new ServiceResponse<string>(null, ApiResponseType.NotFound, e.Message);
        }
    }

    public ServiceResponse<List<GetDataGroupDto>> DisplayDataSet(string username)
    {
        
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        
        var datasets = context.DataSets
            .Include(ds => ds.VertexEntity)
            .Include(ds => ds.EdgeEntity)
            .Include(ds => ds.User)
            .Where(ds=>ds.User.Username==username)
            .ToList();

        var dataGroupDtos = datasets.Select(ds => _mapper.Map<GetDataGroupDto>(ds)).ToList();
        return new ServiceResponse<List<GetDataGroupDto>>(dataGroupDtos, ApiResponseType.Success, "");
    }

    public async Task<ServiceResponse<DisplayGraphDto>> DisplayGeraphData(string databaseName, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName)
    {
        var graph = _displayDataService.GetGraph(databaseName, sourceEdgeIdentifierFieldName, destinationEdgeIdentifierFieldName,
            vertexIdentifierFieldName);

        var dto = new DisplayGraphDto()
        {
            Vertices = graph.vertices,
            Edges = graph.edges,
        };
        return new ServiceResponse<DisplayGraphDto>(dto, ApiResponseType.Success, "");
    }
}