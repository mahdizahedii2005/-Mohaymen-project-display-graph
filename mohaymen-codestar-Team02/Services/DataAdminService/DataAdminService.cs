using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
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

    public DataAdminService(
        ITokenService tokenService,
        ICookieService cookieService,
        IStorHandler storHandler,
        IDisplayDataService displayDataService,
        IEdgeService edgeService,
        IVertexService vertexService)
    {
        _tokenService = tokenService;
        _cookieService = cookieService;
        _vertexService = vertexService;
        _edgeService = edgeService;
        _storHandler = storHandler;
        _displayDataService = displayDataService;
    }

    public async Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? edgeEntityName, string vertexEntityName)
    {
        try
        {
            var token = _cookieService.GetCookieValue();
            if (string.IsNullOrEmpty(token))
            {
                return new ServiceResponse<string>(null, ApiResponseType.Unauthorized,
                    Resources.UnauthorizedMessage);
            }

            var userName = _tokenService.GetUserNameFromToken();
            if (string.IsNullOrEmpty(edgeEntityName) || string.IsNullOrEmpty(graphName) ||
                string.IsNullOrEmpty(vertexEntityName))
                return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest,
                    Data.Resources.InvalidInpute);

            var dataGroupId = await _storHandler.StoreDataSet(graphName, userName);
            if (dataGroupId == -1)
                return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest,
                    Data.Resources.InvalidInpute);

            if (!await _storHandler.EdageStorer.StoreFileData(edgeEntityName, edgeFile, dataGroupId))
                return new ServiceResponse<string>(string.Empty,
                    ApiResponseType.BadRequest, Data.Resources.InvalidInpute);

            if (!await _storHandler.VertexStorer.StoreFileData(vertexEntityName, vertexFile, dataGroupId))
                return new ServiceResponse<string>(string.Empty,
                    ApiResponseType.BadRequest, Data.Resources.InvalidInpute);

            return new ServiceResponse<string>(null, ApiResponseType.Success, string.Empty);
        }
        catch (NullReferenceException e)
        {
            return new ServiceResponse<string>(null, ApiResponseType.NotFound, e.Message);
        }
    }

    public Task<ServiceResponse<string>> DisplayDataSet()
    {
        throw new NotImplementedException();
    }


    public async Task<ServiceResponse<DisplayGraphDto>> DisplayGeraphData(string databaseName,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName)
    {
        var graph = _displayDataService.GetGraph(databaseName, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName,
            vertexIdentifierFieldName);

        var dto = new DisplayGraphDto()
        {
            Vertices = graph.vertices,
            Edges = graph.edges,
        };
        return new ServiceResponse<DisplayGraphDto>(dto, ApiResponseType.Success, "");
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