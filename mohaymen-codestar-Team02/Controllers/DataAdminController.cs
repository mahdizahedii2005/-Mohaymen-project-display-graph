using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.StoreDataDto;
using mohaymen_codestar_Team02.Services.DataAdminService;
using mohaymen_codestar_Team02.Services.FileReaderService;

namespace mohaymen_codestar_Team02.Controllers;

public class DataAdminController : ControllerBase
{
    private readonly IDataAdminService _dataAdminService;
    private readonly IFileReader _fileReader;

    public DataAdminController(IDataAdminService dataAdminService, IFileReader fileReader)
    {
        _dataAdminService = dataAdminService;
        _fileReader = fileReader;
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
    public void DisplayDataSetAsGraph(string dataSetName)
    {
    }

    [HttpGet("DataSets/{dataSetName}/Vertices/{vertexId}")]
    public void DisplayVertexDetails(string datasetName, int vertexId)
    {
    }

    [HttpGet("DataSets/{dataSetName}/Edges/{edgeId}")]
    public void DisplayEdgeDetails(string datasetName, int edgeId)
    {
    }
}