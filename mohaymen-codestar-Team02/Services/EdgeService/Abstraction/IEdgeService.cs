using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public interface IEdgeService
{
    public Dictionary<string, Dictionary<string, string>> GetAllEdges(long dataSetId,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, Dictionary<string, string> edgeAttributeVales);

    DetailDto GetEdgeDetails(string objId);
    public List<GetAttributeDto> GetEdgeAttributes(long edgeEntityId);
}