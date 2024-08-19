using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService(IModelHandler modelHandler, IStorHandler storHandler)
    : IDataAdminService
{
    public async Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile
        , string? nameData, DateTime? updateAt, DateTime? creatTime)
    {
        if (string.IsNullOrEmpty(nameData) || updateAt == null || creatTime == null)
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        if (!storHandler.EdageStorer.StoreEntity(modelHandler.EdgeModelCreator.GetType(edgeFile)))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!storHandler.VertexStorer.StoreEntity(modelHandler.VertexModelCreator.GetType(vertexFile)))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!storHandler.EdageStorer.StoreAttribute(modelHandler.EdgeModelCreator.GetFields(edgeFile)))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!storHandler.VertexStorer.StoreAttribute(modelHandler.VertexModelCreator.GetFields(vertexFile)))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!storHandler.EdageStorer.StoreValues(modelHandler.EdgeModelCreator.GetRecords(edgeFile)))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
        }

        if (!storHandler.VertexStorer.StoreValues(modelHandler.VertexModelCreator.GetRecords(vertexFile)))
        {
            return new ServiceResponse<string>(string.Empty, ApiResponseType.BadRequest, Data.Resources.InvalidInpute);
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