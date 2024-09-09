using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Exception;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.AnalystService;
using mohaymen_codestar_Team02.Services.DataAdminService;

namespace mohaymen_codestar_Team02.Controllers;

public class AnalystController : ControllerBase
{
    private readonly IAnalystService _analystService;

    public AnalystController(IAnalystService analystService)
    {
        _analystService = analystService;
    }

    [HttpGet("Analyst/{vertexId}")]
    public async Task<IActionResult> ExpandVertex([FromQuery] GraphQueryInfoDto graphQueryInfoDto, string vertexId)
    {
        ServiceResponse<DisplayGraphDto> response;
        try
        {
            response = await _analystService.GetTheVertexNeighbor(graphQueryInfoDto, vertexId);
        }
        catch (ProgramException e)
        {
            response = new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.InternalServerError, e.Message);
        }

        return StatusCode((int)response.Type, response);
    }

    [HttpPost("Analyst")]
    public async Task<IActionResult> DisplayDataSetAsGraph([FromBody] FilterGraphDto filterGraphDto)
    {
        ServiceResponse<DisplayGraphDto> response;
        try
        {
            response =
                await _analystService.DisplayGeraphData(filterGraphDto.DatasetId, filterGraphDto.SourceIdentifier,
                    filterGraphDto.TargetIdentifier, filterGraphDto.VertexIdentifier,
                    filterGraphDto.VertexAttributeValues,
                    filterGraphDto.EdgeAttributeValues);
            response.Data.GraphId = filterGraphDto.DatasetId;
        }
        catch (ProgramException e)
        {
            response = new ServiceResponse<DisplayGraphDto>(null, ApiResponseType.InternalServerError, e.Message);
        }

        return StatusCode((int)response.Type, response);
    }

    [HttpGet("Analyst/Vertex/{id}")]
    public IActionResult DisplayVertexAttributes(long id)
    {
        ServiceResponse<List<GetAttributeDto>> response;
        try
        {
            response = _analystService.GetVertexAttributes(id);
            return StatusCode((int)response.Type, response);
        }
        catch (ProgramException e)
        {
            response = new ServiceResponse<List<GetAttributeDto>>(null, ApiResponseType.InternalServerError, e.Message);
        }

        return StatusCode((int)response.Type, response);
    }

    [HttpGet("Analyst/Edge/{id}")]
    public IActionResult DisplayEdgeAttributes(long id)
    {
        ServiceResponse<List<GetAttributeDto>> response;
        try
        {
            response = _analystService.GetEdgeAttributes(id);
        }
        catch (ProgramException e)
        {
            response = new ServiceResponse<List<GetAttributeDto>>(null, ApiResponseType.InternalServerError, e.Message);
        }

        return StatusCode((int)response.Type, response);
    }
}