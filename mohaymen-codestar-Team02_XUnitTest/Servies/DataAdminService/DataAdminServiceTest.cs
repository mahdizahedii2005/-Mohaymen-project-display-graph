using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.DataAdminService;

public class DataAdminServiceTest
{
    private readonly IStorHandler _storHandler;
    private readonly mohaymen_codestar_Team02.Services.DataAdminService.DataAdminService _sut;

    public DataAdminServiceTest()
    {
        _storHandler = Substitute.For<IStorHandler>();
        var modelHandler = Substitute.For<IModelHandler>();
        _sut = new mohaymen_codestar_Team02.Services.DataAdminService.DataAdminService( _storHandler);
        _storHandler.EdageStorer.StoreFileData(Arg.Any<string>(), Arg.Any<long>()).Returns(true);
        _storHandler.VertexStorer.StoreFileData(Arg.Any<string>(), Arg.Any<long>()).Returns(true);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task StoreData_ReturnsBadRequest_WhenNameIsNullOrEmpty(string? name)
    {
        //Arrange
        //Action
        var result = _sut.StoreData("sample", "sample", "mahdddd",name, 8);
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }
    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenCreatingTheDataGroupIsFail()
    {
        //Arrange
        _storHandler.StoreDataSet("mahdddd",8).Returns(-1);
        //Action
        var result = _sut.StoreData("sample", "sample", "mahdddd","name", 8);
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenEdageStorerStoreValuesReturnFalse()
    {
        //Arrange
        _storHandler.EdageStorer.StoreFileData(Arg.Any<string>(), Arg.Any<long>()).Returns(false);
        //Action
        var result = _sut.StoreData("sample", "sample", "test", "mahdddd", 8);
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenVertexStorerStoreValuesReturnFalse()
    {
        // Arrange
        _storHandler.VertexStorer.StoreFileData(Arg.Any<string>(), Arg.Any<long>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", "mamama", 2);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsSuccses_WhenInputAreValid()
    {
        // Arrange
        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", "a", 2);
        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }
}