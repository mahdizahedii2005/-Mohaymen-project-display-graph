using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface IVertexService
{
    List<Vertex> GetAllVertices(string datasetName);
     DetailDto GetVertexDetails(string objId);
}