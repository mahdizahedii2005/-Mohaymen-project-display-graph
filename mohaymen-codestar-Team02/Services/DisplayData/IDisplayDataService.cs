using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface IDisplayDataService
{
    ServiceResponse<string> DisplayGraph();
    ServiceResponse<string> DisplayVertex();
    ServiceResponse<string> DisplayEdge();
}