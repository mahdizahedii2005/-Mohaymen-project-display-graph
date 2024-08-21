using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService
    : IDataAdminService
{
    private readonly IStorHandler storHandler;

    public DataAdminService(IStorHandler storHandler)
    {
        this.storHandler = storHandler;
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

    public Task<ServiceResponse<DisplayGraphDto>> DisplayDataSetAsGraph(string dataSetName, string vertexFieldName
        , string sourceField, string targetField)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<DetailDto>> DisplayVertexData(string dataSetName, long vertexObjectId)
    {
        return null;
    }
    

    public Task<ServiceResponse<DetailDto>> DisplayEdgeData(string setName, long edgeObjectId)
    {
        throw new NotImplementedException();
    }
}