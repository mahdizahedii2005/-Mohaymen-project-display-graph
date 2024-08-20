using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.StoreData;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.StorData;

public class StoreDataServiceTest
{
    private StoreDataService _sut;
    private DataContext _mockContext;
    private ServiceProvider _serviceProvider;

    public StoreDataServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        IEdageStorer edageStorer = Substitute.For<IEdageStorer>();
        IVertexStorer vertexStorer = Substitute.For<IVertexStorer>();
        _sut = new StoreDataService(_serviceProvider, edageStorer, vertexStorer);
      
    }

    [Fact]
    public async Task StoreDataSet_ShouldStoreTheData_whenDataIsVilid()
    {  using var scope = _serviceProvider.CreateScope();
        _mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        //Arrange
        var name = "mahdi";
        _mockContext.Users.Add(new User()
            { Username = "3", UserId = 4, PasswordHash = Array.Empty<byte>(), Salt = Array.Empty<byte>() });
        _mockContext.SaveChanges();
        //Act
        var bolResult = _sut.StoreDataSet(name, "3");
        var result = await _mockContext.DataSets.FirstOrDefaultAsync(x => x.Name == name);
        //Assert
        Assert.True(bolResult.Result != -1);
        Assert.Equal(result.Name, name);
    }

    [Fact]
    public async Task StoreDataSet_ShouldReturnFalse_NameDataIsNull()
    {  using var scope = _serviceProvider.CreateScope();
        _mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        //Arrange
        //Act 
        var result = _sut.StoreDataSet(null, "3");
        //Assert
        Assert.True(result.Result == -1);
        Assert.Empty(_mockContext.DataSets);
    }
}