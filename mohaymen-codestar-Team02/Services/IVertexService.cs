using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface IVertexService
{
    public List<Vertex> GetAllVertices(long dataSetId, string vertexIdentifierFieldName);
    DetailDto GetVertexDetails(string objId);
    public List<GetAttributeDto> GetVertexAttributes(long vertexEntityId);
}