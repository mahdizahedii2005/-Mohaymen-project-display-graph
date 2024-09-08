namespace mohaymen_codestar_Team02.Dto;

public class GetGraphDto
{
    public long DatasetId { get; set; }
    public string SourceIdentifier { get; set; }
    public string TargetIdentifier { get; set; }
    public string VertexIdentifier { get; set; }
}