using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Services.AnalystService;

namespace mohaymen_codestar_Team02.Controllers;

public class AnalystController : ControllerBase
{
    private readonly IAnalystService _analystService;

    [HttpGet("Analyst/{vertexId}")]
    public async Task<IActionResult> ExpandVertex([FromQuery] GraphQueryInfoDto graphQueryInfoDto, string vertexId)
    {
        var Response = await _analystService.GetTheVertexNeighbor(graphQueryInfoDto, vertexId);
        return StatusCode((int)Response.Type, Response);
    }

    [HttpPost("Analyst")]
    public async Task<IActionResult> DisplayDataSetAsGraph([FromBody] FilterGraphDto filterGraphDto)
    {
        var response =
            await _analystService.DisplayGeraphData(filterGraphDto.DatasetId, filterGraphDto.SourceIdentifier,
                filterGraphDto.TargetIdentifier, filterGraphDto.VertexIdentifier, filterGraphDto.VertexAttributeValues,
                filterGraphDto.EdgeAttributeValues);
        response.Data.GraphId = filterGraphDto.DatasetId;
        return StatusCode((int)response.Type, response);
    }

    [HttpGet("Analyst/Vertex/{id}")]
    public IActionResult DisplayVertexAttributes(long id)
    {
        var response = _analystService.GetVertexAttributes(id);
        return StatusCode((int)response.Type, response);
    }

    [HttpGet("Analyst/Edge/{id}")]
    public IActionResult DisplayEdgeAttributes(long id)
    {
        var response = _analystService.GetEdgeAttributes(id);
        return StatusCode((int)response.Type, response);
    }
}