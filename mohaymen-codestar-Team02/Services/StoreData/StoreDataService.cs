using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using DataSet = mohaymen_codestar_Team02.Models.DataSet;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class StoreDataService(DataContext context, IEdageStorer edageStorer, IVertexStorer vertexStorer) : IStorHandler
{
    public IEdageStorer EdageStorer { get; set; } = edageStorer;
    public IVertexStorer VertexStorer { get; set; } = vertexStorer;

    public bool StoreDataSet(string? nameData)
    {
        var dateTime = DateTime.Now;
        if (string.IsNullOrEmpty(nameData)) return false;
        var setData = new DataSet();
    }
}