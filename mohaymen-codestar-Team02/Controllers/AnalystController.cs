using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Services.AnalystService;

namespace mohaymen_codestar_Team02.Controllers;

public class AnalystController : ControllerBase
{
    private readonly IAnalystService AnalystService;

    public AnalystController(IAnalystService analystService)
    {
        AnalystService = analystService;
    }

    [HttpGet("Analyst")]
    public Task<IActionResult> SearchGraph([FromQuery] GraphQueryInfoDto graphQueryInfoDto, [FromQuery] Dictionary<string, string> vertexAttributeValues)
    {
        return null;
    }
    
    [HttpGet("Analyst/{vertexId}")]
    public Task<IActionResult> ExpandVertex([FromQuery] GraphQueryInfoDto graphQueryInfoDto, string vertexId)
    {
        return null;
    }

}