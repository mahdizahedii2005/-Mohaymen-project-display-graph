using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface IVertexService
{
    public Dictionary<string, Dictionary<string, string>> GetAllVertices(long dataSetId, string vertexIdentifierFieldName, Dictionary<string, string> vertexAttributeVales);
    DetailDto GetVertexDetails(string objId);
    public List<GetAttributeDto> GetVertexAttributes(long vertexEntityId);

}