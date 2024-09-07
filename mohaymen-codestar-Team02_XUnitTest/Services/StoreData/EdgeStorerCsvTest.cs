using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.StoreData;

namespace mohaymen_codestar_Team02_XUnitTest.Services.StoreData;

public class EdgeStorerCsvTests
{
    private EdgeStorerCsv _sut;
    private DataContext _dataContext;
    private ServiceProvider _serviceProvider;

    public EdgeStorerCsvTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new EdgeStorerCsv(_serviceProvider);
        using var scope = _serviceProvider.CreateScope();
        _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    }

    [Fact]
    public async Task StoreFileData_ShouldReturnTrue_WhenDataIsValid()
    {
        using var scope = _serviceProvider.CreateScope();
        _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        var entityName = "TestEntity";
        var dataFile = "attribute1,attribute2\nvalue1,value2\nlol1,lol2";
        var dataGroupId = 1;

        // Act
        var result = await _sut.StoreFileData(entityName, dataFile, dataGroupId);

        // Assert
        Assert.True(result);
        Assert.Equal(1, await _dataContext.EdgeEntities.CountAsync());
        Assert.Equal(1, await _dataContext.EdgeAttributes.CountAsync());
        Assert.Equal(2, await _dataContext.EdgeValues.CountAsync());
    }

    [Fact]
    public async Task StoreFileData_ShouldReturnFalse_WhenHeaderLineIsNull()
    {
        using var scope = _serviceProvider.CreateScope();
        _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        // Arrange
        var entityName = "TestEntity";
        var dataFile = ""; // No headers in the file
        var dataGroupId = 1;

        // Act
        var result = await _sut.StoreFileData(entityName, dataFile, dataGroupId);

        // Assert
        Assert.False(result);
        Assert.Equal(1, await _dataContext.EdgeEntities.CountAsync());
        Assert.Equal(0, await _dataContext.EdgeAttributes.CountAsync());
        Assert.Equal(0, await _dataContext.EdgeValues.CountAsync());
    }
}