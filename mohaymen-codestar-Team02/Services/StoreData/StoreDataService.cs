using System.Dynamic;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class StoreDataService(DataContext context, IEdageStorer edageStorer, IVertexStorer vertexStorer) : IStorHandler
{
    public IEdageStorer EdageStorer { get; set; } = edageStorer;
    public IVertexStorer VertexStorer { get; set; } = vertexStorer;

    public bool StoreDataSet(string? nameData, DateTime? updateAt, DateTime? creatTime)
    {
        throw new NotImplementedException();
    }
}