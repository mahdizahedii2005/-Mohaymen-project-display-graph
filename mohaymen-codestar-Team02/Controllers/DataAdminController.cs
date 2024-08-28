using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Dto.StoreDataDto;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.DataAdminService;
using mohaymen_codestar_Team02.Services.FileReaderService;

namespace mohaymen_codestar_Team02.Controllers;

public class DataAdminController : ControllerBase
{
    private readonly IDataAdminService _dataAdminService;
    private readonly IFileReader _fileReader;

    public DataAdminController(IDataAdminService dataAdminService,
        IFileReader fileReader, IGraphService graphService)
    {
        _dataAdminService = dataAdminService;
        _fileReader = fileReader;
        _dataAdminService = dataAdminService;
    }

    [HttpPost("DataSets")]
    public async Task<IActionResult> StoreNewDataSet([FromForm] StoreDataDto storeDataDto)
    {
        //Todo SystemAdmin and DataAdmin
        try
        {
            var edgeFile = _fileReader.Read(storeDataDto.EdgeFile);
            var vertexFile = _fileReader.Read(storeDataDto.VertexFile);
            var response = await _dataAdminService.StoreData(edgeFile, vertexFile, storeDataDto.DataName,
                Path.GetFileName(storeDataDto.EdgeFile.FileName), Path.GetFileName(storeDataDto.VertexFile.FileName));
            return StatusCode((int)response.Type, response);
        }
        catch (FormatException e)
        {
            return BadRequest("your File is not on a correct format");
        }
    }

    [HttpGet("DataSets")]
    public IActionResult GetDataSetsList()
    {
        var response = _dataAdminService.DisplayDataSet();
        return StatusCode((int)response.Type, response);

        //Todo all Of Them
    }

    [HttpGet("DataSets/{dataSetId}")]
    public async Task<IActionResult> DisplayDataSetAsGraph([FromQuery]GraphQueryInfoDto graphQueryInfoDto, [FromQuery] Dictionary<string, string> vertexAttributeValues, [FromQuery] Dictionary<string, string> edgeAttributeValues)
    {
        //Todo all Of Them
        ServiceResponse<DisplayGraphDto> response =
            await _dataAdminService.DisplayGeraphData(graphQueryInfoDto.datasetId, graphQueryInfoDto.sourceIdentifier,
                graphQueryInfoDto.targetIdentifier, graphQueryInfoDto.vertexIdentifier, vertexAttributeValues, edgeAttributeValues);
        response.Data.GraphId = graphQueryInfoDto.datasetId;
        return StatusCode((int)response.Type, response);
    }


    [HttpGet("DataSets/Vertices/{objectId}")]
    public async Task<IActionResult> DisplayVertexDetails(string objectId)
    {
        //Todo all Of Them

        var respond = _dataAdminService.GetVertexDetail(objectId);
        return StatusCode((int)respond.Type, respond);
    }

    [HttpGet("DataSets/Edges/{objectId}")]
    public async Task<IActionResult> DisplayEdgeDetails(string objectId)
    {
        //Todo all Of Them
        var respond = _dataAdminService.GetEdgeDetail(objectId);
        return StatusCode((int)respond.Type, respond);
    }

    [HttpGet("DataSets/Vertex/{id}")]
    public async Task<IActionResult> DisplayVertexAttributes(long id)
    {
        var response = _dataAdminService.GetVertexAttributes(id);
        return StatusCode((int)response.Type, response);
    }

    [HttpGet("DataSets/Edge/{id}")]
    public async Task<IActionResult> DisplayEdgeAttributes(long id)
    {
        var response = _dataAdminService.GetEdgeAttributes(id);
        return StatusCode((int)response.Type, response);
    }
}