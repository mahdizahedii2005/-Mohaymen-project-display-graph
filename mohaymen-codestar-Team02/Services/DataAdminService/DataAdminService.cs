using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService(IStorHandler storHandler)
    : IDataAdminService
{
    public async Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? nameData, long userId)
    {
        if (string.IsNullOrEmpty(nameData)||string.IsNullOrEmpty(graphName))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        var dataGroupId = storHandler.StoreDataSet(graphName, userId);
        if (dataGroupId == -1)
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }
        if (!storHandler.EdageStorer.StoreFileData(edgeFile, dataGroupId))
        {
            return new ServiceResponse<string>(string.Empty,
                ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!storHandler.VertexStorer.StoreFileData(vertexFile, dataGroupId))
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