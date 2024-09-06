using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.AnalystService;
using mohaymen_codestar_Team02.Services.DataAdminService;

namespace mohaymen_codestar_Team02.Controllers;

public class AnalystController : ControllerBase
{
    private readonly IAnalystService AnalystService;
    private readonly IDataAdminService _dataAdminService;

    public AnalystController(IAnalystService analystService, IDataAdminService dataAdminService)
    {
        AnalystService = analystService;
        _dataAdminService = dataAdminService;
    }

    [HttpGet("Analyst")]
    public Task<IActionResult> SearchGraph([FromQuery] GraphQueryInfoDto graphQueryInfoDto,
        [FromQuery] Dictionary<string, string> vertexAttributeValues)
    {
        return null;
    }

    [HttpGet("Analyst/{vertexId}")]
    public async Task<IActionResult> ExpandVertex([FromQuery] GraphQueryInfoDto graphQueryInfoDto, string vertexId)
    {
        var Response = await AnalystService.GetTheVertexNeighbor(graphQueryInfoDto, vertexId);
        return StatusCode((int)Response.Type, Response);
    }
    
    [HttpPost("Analyst")]
    public async Task<IActionResult> DisplayDataSetAsGraph([FromBody]FilterGraphDto filterGraphDto)
    {
        ServiceResponse<DisplayGraphDto> response =
            await _dataAdminService.DisplayGeraphData(filterGraphDto.DatasetId, filterGraphDto.SourceIdentifier,
                filterGraphDto.TargetIdentifier, filterGraphDto.VertexIdentifier, filterGraphDto.VertexAttributeValues, filterGraphDto.EdgeAttributeValues);
        response.Data.GraphId = filterGraphDto.DatasetId;
        return StatusCode((int)response.Type, response);
    }
    
    [HttpGet("Analyst/Vertex/{id}")]
    public async Task<IActionResult> DisplayVertexAttributes(long id)
    {
        var response = _dataAdminService.GetVertexAttributes(id);
        return StatusCode((int)response.Type, response);
    }

    [HttpGet("Analyst/Edge/{id}")]
    public async Task<IActionResult> DisplayEdgeAttributes(long id)
    {
        var response = _dataAdminService.GetEdgeAttributes(id);
        return StatusCode((int)response.Type, response);
    }
}