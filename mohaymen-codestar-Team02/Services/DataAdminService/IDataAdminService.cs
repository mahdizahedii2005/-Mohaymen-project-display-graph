using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public interface IDataAdminService
{
    Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? edgeEntityName, string vertexEntityName, string userName);

    Task<ServiceResponse<DisplayGraphDto>> DisplayDataSetAsGraph(string dataSetName, string vertexFieldName
        , string sourceField, string targetField);
    Task<ServiceResponse<string>> DisplayDataSet();

    Task<ServiceResponse<List<Vertex>>> DisplayVertexData(string databaseName, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName);
    Task<ServiceResponse<List<Edge>>> DisplayEdgeData();
    
}