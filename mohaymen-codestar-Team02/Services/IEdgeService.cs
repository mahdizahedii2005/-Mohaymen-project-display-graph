using mohaymen_codestar_Team02.Dto.GraphDTO;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public interface IEdgeService
{
    List<Edge<string>> GetAllEdges(string datasetName);
    DetailDto GetEdgeDetails(string objId);
}