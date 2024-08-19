using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class StoreDataService(DataContext context, IEdageStorer edageStorer, IVertexStorer vertexStorer) : IStorHandler
{
    public IEdageStorer EdageStorer { get; set; } = edageStorer;
    public IVertexStorer VertexStorer { get; set; } = vertexStorer;

    public bool StoreDataSet(string? nameData, int userId)
    {
        try
        {
            if (string.IsNullOrEmpty(nameData)) return false;
            var setData = new DataGroup(nameData, userId);
            context.DataSets.Add(setData);
            context.SaveChanges();
            return true;
        }
        catch (ArgumentException e)
        {
            return false;
        }
    }
}