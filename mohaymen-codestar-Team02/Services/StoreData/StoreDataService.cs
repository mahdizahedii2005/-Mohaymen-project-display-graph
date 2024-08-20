using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class StoreDataService
    : IStorHandler
{
    private readonly IServiceProvider _serviceProvider;
   
    public StoreDataService(IServiceProvider serviceProvider, IEdageStorer edageStorer, IVertexStorer vertexStorer)
    {
        _serviceProvider = serviceProvider;
        VertexStorer = vertexStorer;
        EdageStorer = edageStorer;
    }
    public IEdageStorer EdageStorer { get; set; }
    public IVertexStorer VertexStorer { get; set; }

    public async Task<long> StoreDataSet(string? nameData, string userName)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        try
        {
            if (string.IsNullOrEmpty(nameData)) return -1;
            var setData = new DataGroup(nameData,
                context.Users.SingleOrDefault(u => u.Username.ToLower() == userName.ToLower()).UserId);
            await context.DataSets.AddAsync(setData);
            await context.SaveChangesAsync();
            return setData.DataGroupId;
        }
        catch (ArgumentException e)
        {
            return -1;
        }
    }
}