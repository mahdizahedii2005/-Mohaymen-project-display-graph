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
    private ServiceProvider _serviceProvider;

    public StoreDataServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        var edageStorer = Substitute.For<IEdageStorer>();
        var vertexStorer = Substitute.For<IVertexStorer>();
        _sut = new StoreDataService(_serviceProvider, edageStorer, vertexStorer);
    }

    [Fact]
    public async Task StoreDataSet_ShouldStoreTheData_whenDataIsVilid()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        //Arrange
        var name = "mahdi";
        mockContext.Users.Add(new User()
            { Username = "3", UserId = 4, PasswordHash = Array.Empty<byte>(), Salt = Array.Empty<byte>() });
        mockContext.SaveChanges();
        //Act
        var bolResult = await _sut.StoreDataSet(name, "3");
        var result = await mockContext.DataSets.FirstOrDefaultAsync(x => x.Name == name);
        //Assert
        Assert.True(bolResult != -1);
        Assert.Equal(result?.Name, name);
    }

    [Fact]
    public async Task StoreDataSet_ShouldReturnFalse_NameDataIsNull()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        //Arrange
        //Act 
        var result = await _sut.StoreDataSet(null, "3");
        //Assert
        Assert.True(result == -1);
        Assert.Empty(mockContext.DataSets);
    }
}