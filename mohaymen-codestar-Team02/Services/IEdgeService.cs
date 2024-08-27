using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public interface IEdgeService
{
    public List<Edge> GetAllEdges(long dataSetId, string vertexIdentifierFieldName,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName);

    DetailDto GetEdgeDetails(string objId);
    public List<GetAttributeDto> GetEdgeAttributes(long edgeEntityId);
}