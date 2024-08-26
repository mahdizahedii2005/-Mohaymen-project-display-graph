using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;

namespace mohaymen_codestar_Team02.Dto;

public class GetDataGroupDto
{
    public string Name { get; set; }
    
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    
    public virtual GetEdgeEntityDto EdgeEntity { get; set; }
    public virtual GetVertexEntityDto VertexEntity { get; set; }
}