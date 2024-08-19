using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService(IStorHandler storHandler)
    : IDataAdminService
{
    public async Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? edgeEntityName, string vertexEntityName, string userName)
    {
        if (string.IsNullOrEmpty(edgeEntityName) || string.IsNullOrEmpty(graphName) ||
            string.IsNullOrEmpty(vertexEntityName))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        var dataGroupId = storHandler.StoreDataSet(graphName, userName);
        if (string.IsNullOrEmpty(dataGroupId))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!await storHandler.EdageStorer.StoreFileData(edgeEntityName, edgeFile, dataGroupId))
        {
            return new ServiceResponse<string>(string.Empty,
                ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!await storHandler.VertexStorer.StoreFileData(vertexEntityName, vertexFile, dataGroupId))
        {
            return new ServiceResponse<string>(string.Empty,
                ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        return new ServiceResponse<string>(null, ApiResponseType.Success, string.Empty);
    }

    public Task<ServiceResponse<string>> DisplayDataSet()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<string>> DisplayVertexData()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<string>> DisplayEdgeData()
    {
        throw new NotImplementedException();
    }
}