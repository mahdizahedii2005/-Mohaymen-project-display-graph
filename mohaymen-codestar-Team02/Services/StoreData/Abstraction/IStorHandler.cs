namespace mohaymen_codestar_Team02.Services.StoreData.Abstraction;

public interface IStorHandler
{
    public IEdageStorer EdageStorer { get; set; }
    public IVertexStorer VertexStorer { get; set; }
    public string StoreDataSet(string? nameData, string userId);
}