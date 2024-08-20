using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class StoreDataService(IServiceProvider serviceProvider, IEdageStorer edageStorer, IVertexStorer vertexStorer) : IStorHandler
{
    public IEdageStorer EdageStorer { get; set; } = edageStorer;
    public IVertexStorer VertexStorer { get; set; } = vertexStorer;
    public long StoreDataSet(string? nameData, string userName)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        try
        {
            if (string.IsNullOrEmpty(nameData)) return -1;
            var setData = new DataGroup(nameData,
                context.Users.SingleOrDefault(u => u.Username.ToLower() == userName.ToLower()).UserId);
            context.DataSets.Add(setData);
            context.SaveChanges();
            return setData.Id;
        }
        catch (ArgumentException e)
        {
            return -1;
        }
    }
}