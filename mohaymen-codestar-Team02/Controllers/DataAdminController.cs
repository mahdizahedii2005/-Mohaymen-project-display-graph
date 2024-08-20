using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.StoreDataDto;
using mohaymen_codestar_Team02.Services.DataAdminService;
using mohaymen_codestar_Team02.Services.FileReaderService;

namespace mohaymen_codestar_Team02.Controllers;

public class DataAdminController(IDataAdminService _dataAdminService, IFileReader fileReader) : ControllerBase
{
    [HttpPost("DataSets")]
    public async Task<IActionResult> StoreNewDataSet([FromForm] StoreDataDto storeDataDto)
    {
        var edgeFile = fileReader.Read(storeDataDto.EdgeFile);
        var vertexFile = fileReader.Read(storeDataDto.VertexFile);
        var response = await _dataAdminService.StoreData(edgeFile, vertexFile, storeDataDto.DataName,
            Path.GetFileName(storeDataDto.EdgeFile.FileName), Path.GetFileName(storeDataDto.VertexFile.FileName),
            storeDataDto.CreatorUserName);
        return StatusCode((int)response.Type, response);
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