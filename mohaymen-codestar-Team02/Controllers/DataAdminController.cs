using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.StoreDataDto;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.DataAdminService;

namespace mohaymen_codestar_Team02.Controllers;

public class DataAdminController
{
    private readonly IDataAdminService _storeService;

    public DataAdminController(IDataAdminService storeService)
    {
        _storeService = storeService;
    }


    [HttpPost("StoreNewDataSet")]
    public void StoreNewDataSet([FromBody] StoreDataDto storeDataDto)
    {
        
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