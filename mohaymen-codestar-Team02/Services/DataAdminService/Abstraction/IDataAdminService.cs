using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public interface IDataAdminService
{
    Task<ServiceResponse<string>> StoreData(string? edgeFile, string? vertexFile, string graphName
        , string? edgeEntityName, string vertexEntityName);

    ServiceResponse<List<GetDataGroupDto>> DisplayDataSet();

    Task<ServiceResponse<DisplayGraphDto>> DisplayGeraphData(long dataSetId, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName,
        Dictionary<string, string> vertexAttributeVales, Dictionary<string, string> edgeAttributeVales);

    ServiceResponse<DetailDto> GetVertexDetail(string objectId);
    ServiceResponse<DetailDto> GetEdgeDetail(string objectId);

    ServiceResponse<List<GetAttributeDto>> GetVertexAttributes(long vertexEntityId);

    ServiceResponse<List<GetAttributeDto>> GetEdgeAttributes(long edgeEntityId);
}