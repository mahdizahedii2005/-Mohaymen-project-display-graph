using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.StoreDataDto;
using mohaymen_codestar_Team02.Services.DataAdminService;
using mohaymen_codestar_Team02.Services.FileReaderService;

namespace mohaymen_codestar_Team02.Controllers;

public class DataAdminController(IDataAdminService _dataAdminService, IFileReader fileReader)
{
    [HttpPost("StoreNewDataSet")]
    public void StoreNewDataSet([FromBody] StoreDataDto storeDataDto)
    {
        var edgeFile = fileReader.Read(storeDataDto.EdgeFile);
        var vertexFile = fileReader.Read(storeDataDto.VertexFile);
        _dataAdminService.StoreData(edgeFile, vertexFile, storeDataDto.DataName,
            Path.GetFileName(storeDataDto.EdgeFile.FileName), Path.GetFileName(storeDataDto.VertexFile.FileName),
            storeDataDto.CreatorUserName);
    }

    [HttpGet("GetDataSetsList")]
    public void GetDataSetsList()
    {
    }

    [HttpGet("{dataSetName}")]
    public void DisplayDataSetAsGraph(string dataSetName)
    {
    }

    [HttpGet("{datasetName, vertexId}")]
    public void DisplayVertexDetails(string datasetName, int vertexId)
    {
    }

    [HttpGet("{datasetName, edgeId}")]
    public void DisplayEdgeDetails(string datasetName, string edgeId)
    {
    }
}