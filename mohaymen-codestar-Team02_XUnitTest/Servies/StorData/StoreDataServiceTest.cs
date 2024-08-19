using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.StoreData;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.StorData;

public class StoreDataServiceTest
{
    private DataContext _mockContext;
    private StoreDataService _sut;

    public StoreDataServiceTest()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: new Random().NextInt64() + Guid.NewGuid().ToString())
            .Options;
        _mockContext = new DataContext(options);
        IEdageStorer edageStorer = Substitute.For<IEdageStorer>();
        IVertexStorer vertexStorer = Substitute.For<IVertexStorer>();
        _sut = new StoreDataService(_mockContext, edageStorer, vertexStorer);
    }

    [Fact]
    public async Task StoreDataSet_ShouldStoreTheData_whenDataIsVilid()
    {
        //Arrange
        var dateTime = DateTime.Now;
        var name = "mahdi";
        //Act
        var bolResult = _sut.StoreDataSet(name);
        var result = await _mockContext.DataSets.FirstOrDefaultAsync(x => x.Name == name);
        //Assert
        Assert.True(bolResult);
        Assert.NotEqual(result.Name, name);
        Assert.Contains(name, result.Name);
        Assert.Equal(result.CreateAt, dateTime);
        Assert.Equal(result.UpdateAt, dateTime);
    }

    [Fact]
    public async Task StoreDataSet_ShouldReaturnFalse_NameDataIsNull()
    {
        //Arrange
        //Act 
        var result = _sut.StoreDataSet(null);
        //Assert
        Assert.False(result);
        Assert.Empty(_mockContext.DataSets);
    }
}