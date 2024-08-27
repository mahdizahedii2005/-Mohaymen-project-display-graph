using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;

namespace mohaymen_codestar_Team02.Services.AnalystService;

public interface IAnalystService
{
    DisplayGraphDto GetTheVertexNeighbor(GraphQueryInfoDto graphQueryInfoDto, string vertexId);
}