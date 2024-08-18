using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ModelData;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminService(IModelDataService modelDataService,IStoreDataService storeDataService) 
    : IDataAdminService
{
    public Task<ServiceResponse<string>> StoreData(string edgeFile, string vertexFile
        , string nameData, DateTime updateAt, DateTime creatTime)
    {
        
        
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