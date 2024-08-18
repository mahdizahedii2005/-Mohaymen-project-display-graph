using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface IDataAdminService
{
    Task<ServiceResponse<string>> StoreData();
    Task<ServiceResponse<string>> DisplayDataSet();
    Task<ServiceResponse<string>> DisplayVertexData();
    Task<ServiceResponse<string>> DisplayEdgeData();
}