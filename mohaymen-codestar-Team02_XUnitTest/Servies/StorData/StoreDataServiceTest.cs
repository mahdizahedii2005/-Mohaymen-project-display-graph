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
        var name = "mahdi";
        //Act
        var bolResult = _sut.StoreDataSet(name,"3");
        var result = await _mockContext.DataSets.FirstOrDefaultAsync(x => x.Name == name);
        //Assert
        Assert.NotEmpty(bolResult);
        Assert.Equal(result.Name, name);
    }

    [Fact]
    public async Task StoreDataSet_ShouldReturnFalse_NameDataIsNull()
    {
        //Arrange
        //Act 
        var result = _sut.StoreDataSet(null,"3");
        //Assert
        Assert.Empty(result);
        Assert.Empty(_mockContext.DataSets);
    }
}