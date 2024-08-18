using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Services;

namespace mohaymen_codestar_Team02.Controllers;

public class DataAdminController
{
    
    private readonly IDataAdminService _storeService;

    public DataAdminController(IDataAdminService storeService)
    {
        _storeService = storeService;
    }

    
    [HttpPost("StoreNewDataSet")]
    public void StoreNewDataSet(SaveDataDto saveDataDto1, SaveDataDto saveDataDto2)
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