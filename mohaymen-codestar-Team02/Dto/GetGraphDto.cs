namespace mohaymen_codestar_Team02.Dto;

public class GetGraphDto
{
    public long datasetId { get; set; }
    public string sourceIdentifier { get; set; }
    public string targetIdentifier { get; set; }
    public string vertexIdentifier { get; set; } 
    public Dictionary<string, string> vertexAttributeValues { get; set; }
    public Dictionary<string, string> edgeAttributeValues { get; set; }
}