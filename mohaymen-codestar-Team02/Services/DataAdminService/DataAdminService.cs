using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService
    : IDataAdminService
{
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IEdgeService _edgeService;
    private readonly IVertexService _vertexService;
    private readonly IStorHandler _storHandler;
    private readonly IDisplayDataService _displayDataService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly IGraphService _graphService;

    public DataAdminService(
        IServiceProvider serviceProvider,
        ITokenService tokenService,
        ICookieService cookieService,
        IStorHandler storHandler,
        IDisplayDataService displayDataService,
        IEdgeService edgeService,
        IVertexService vertexService, IMapper mapper, IGraphService graphService)
    {
        _tokenService = tokenService;
        _cookieService = cookieService;
        _vertexService = vertexService;
        _edgeService = edgeService;
        _storHandler = storHandler;
        _displayDataService = displayDataService;
        _mapper = mapper;
        _graphService = graphService;
        _serviceProvider = serviceProvider;
    }

    public async Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? edgeEntityName, string vertexEntityName)
    {
        try
        {
            var token = _cookieService.GetCookieValue();
            if (string.IsNullOrEmpty(token))
                return new ServiceResponse<string>(null, ApiResponseType.Unauthorized,
                    Resources.UnauthorizedMessage);

            var userName = _tokenService.GetUserNameFromToken();
            if (string.IsNullOrEmpty(edgeEntityName) || string.IsNullOrEmpty(graphName) ||
                string.IsNullOrEmpty(vertexEntityName))
                return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest,
                    Resources.InvalidInputeMessage);

            var dataGroupId = await _storHandler.StoreDataSet(graphName, userName);
            if (dataGroupId == -1)
                return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest,
                    Resources.InvalidInputeMessage);

            if (!await _storHandler.EdageStorer.StoreFileData(edgeEntityName, edgeFile, dataGroupId))
                return new ServiceResponse<string>(string.Empty,
                    ApiResponseType.BadRequest, Resources.InvalidInputeMessage);

            if (!await _storHandler.VertexStorer.StoreFileData(vertexEntityName, vertexFile, dataGroupId))
                return new ServiceResponse<string>(string.Empty,
                    ApiResponseType.BadRequest, Resources.InvalidInputeMessage);

            return new ServiceResponse<string>(null, ApiResponseType.Success, string.Empty);
        }
        catch (NullReferenceException e)
        {
            return new ServiceResponse<string>(null, ApiResponseType.NotFound, e.Message);
        }
    }

    public ServiceResponse<List<GetDataGroupDto>> DisplayDataSet()
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var datasets = context.DataSets
            .Include(ds => ds.VertexEntity)
            .Include(ds => ds.EdgeEntity)
            .ToList();

        var dataGroupDtos = datasets.Select(ds => _mapper.Map<GetDataGroupDto>(ds)).ToList();
        return new ServiceResponse<List<GetDataGroupDto>>(dataGroupDtos, ApiResponseType.Success, "");
    }

    public async Task<ServiceResponse<DisplayGraphDto>> DisplayGeraphData(string databaseName,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName)
    {
        var graph = _graphService.GetGraph(databaseName, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName,
            vertexIdentifierFieldName);

        var dto = new DisplayGraphDto()
        {
            Vertices = graph.vertices,
            Edges = graph.edges
        };
        return new ServiceResponse<DisplayGraphDto>(dto, ApiResponseType.Success,
            Resources.GraphFetchedSuccessfullyMessage);
    }

    public ServiceResponse<DetailDto> GetVertexDetail(string objectId)
    {
        return new ServiceResponse<DetailDto>(_vertexService.GetVertexDetails(objectId), ApiResponseType.Success,
            string.Empty);
    }

    public ServiceResponse<DetailDto> GetEdgeDetail(string objectId)
    {
        return new ServiceResponse<DetailDto>(_edgeService.GetEdgeDetails(objectId), ApiResponseType.Success,
            string.Empty);
    }
}