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
    private readonly IDisplayDataService _dataService;

    public DataAdminController(IDataAdminService dataAdminService, IFileReader fileReader,
        IDisplayDataService dataService)
    {
        _dataAdminService = dataAdminService;
        _fileReader = fileReader;
        _dataAdminService = dataAdminService;
    }

    [HttpPost("DataSets")]
    public async Task<IActionResult> StoreNewDataSet([FromForm] StoreDataDto storeDataDto)
    {
        try
        {
            var edgeFile = _fileReader.Read(storeDataDto.EdgeFile);
            var vertexFile = _fileReader.Read(storeDataDto.VertexFile);
            var response = await _dataAdminService.StoreData(edgeFile, vertexFile, storeDataDto.DataName,
                Path.GetFileName(storeDataDto.EdgeFile.FileName), Path.GetFileName(storeDataDto.VertexFile.FileName),
                storeDataDto.CreatorUserName);
            return StatusCode((int)response.Type, response);
        }
        catch (FormatException e)
        {
            return BadRequest("your File is not on a correct format");
        }
    }

    [HttpGet("DataSets")]
    public void GetDataSetsList()
    {
    }

    [HttpGet("DataSets/{dataSetName}")]
    public async Task<IActionResult> DisplayDataSetAsGraph(string dataSetName,
        [FromQuery] string sourceEdgeIdentifierFieldName, [FromQuery] string destinationEdgeIdentifierFieldName,
        [FromQuery] string vertexIdentifierFieldName)
    {
        ServiceResponse<DisplayGraphDto> response =
            await _dataAdminService.DisplayGeraphData(dataSetName, sourceEdgeIdentifierFieldName,
                destinationEdgeIdentifierFieldName, vertexIdentifierFieldName);
        response.Data.GraphName = dataSetName;
        return StatusCode((int)response.Type, response);
    }


    [HttpGet("DataSets/{dataSetName}/Vertices/{vertexId}")]
    public async Task<IActionResult> DisplayVertexDetails(string datasetName, int vertexId)
    {
        return BadRequest();
    }

    [HttpGet("DataSets/{dataSetName}/Edges/{edgeId}")]
    public async Task<IActionResult> DisplayEdgeDetails(string datasetName, int edgeId)
    {
        return BadRequest();
    }
}