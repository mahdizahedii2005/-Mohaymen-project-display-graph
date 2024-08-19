using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class StoreDataService(DataContext context, IEdageStorer edageStorer, IVertexStorer vertexStorer) : IStorHandler
{
    public IEdageStorer EdageStorer { get; set; } = edageStorer;
    public IVertexStorer VertexStorer { get; set; } = vertexStorer;
    public string StoreDataSet(string? nameData, string userName)
    {
        try
        {
            if (string.IsNullOrEmpty(nameData)) return null;
            var setData = new DataGroup(nameData,
                context.Users.SingleOrDefault(u => u.Username.ToLower() == userName.ToLower()).UserId);
            context.DataSets.Add(setData);
            context.SaveChanges();
            return setData.Id;
        }
        catch (ArgumentException e)
        {
            return null;
        }
    }
}