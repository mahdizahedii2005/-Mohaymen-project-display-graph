namespace mohaymen_codestar_Team02.Dto.StoreDataDto;

public class StoreDataDto
{
    public IFormFile EdgeFile { get; set; }
    public IFormFile VertexFile { get; set; }
    public string FileType { get; set; }
    public string DataName { get; set; }

    public string CreatorUserName { get; set; }
}