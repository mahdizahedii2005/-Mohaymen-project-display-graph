using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public interface IDataAdminService
{
    Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? nameData, long userId);

    Task<ServiceResponse<string>> DisplayDataSet();
    Task<ServiceResponse<string>> DisplayVertexData();
    Task<ServiceResponse<string>> DisplayEdgeData();
}