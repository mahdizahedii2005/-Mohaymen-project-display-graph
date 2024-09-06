using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.AnalystService;

public interface IAnalystService
{
    Task<ServiceResponse<DisplayGraphDto>> GetTheVertexNeighbor(GraphQueryInfoDto graphQueryInfoDto, string vertexId);

    Task<ServiceResponse<DisplayGraphDto>> DisplayGeraphData(long databaseId,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName,
        Dictionary<string, string> vertexAttributeValus, Dictionary<string, string> edgeAttributeValues);
    
    ServiceResponse<List<GetAttributeDto>> GetVertexAttributes(long vertexEntityId);

    ServiceResponse<List<GetAttributeDto>> GetEdgeAttributes(long edgeEntityId);
}