using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.DataAdminService;

public class DataAdminServiceTest
{
    private readonly IStorHandler _storHandler;
    private readonly IDisplayDataService _displayDataService;
    private readonly mohaymen_codestar_Team02.Services.DataAdminService.DataAdminService _sut;
    private readonly IEdgeService _edgeService;
    private readonly IVertexService _vertexService;

    public DataAdminServiceTest()
    {
        _vertexService = Substitute.For<IVertexService>();
        _edgeService = Substitute.For<IEdgeService>();
        _storHandler = Substitute.For<IStorHandler>();
        _displayDataService = Substitute.For<IDisplayDataService>();
        _sut = new mohaymen_codestar_Team02.Services.DataAdminService.DataAdminService(_storHandler,
            _displayDataService, _edgeService, _vertexService);
        _storHandler.EdageStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(true);
        _storHandler.VertexStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(true);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task StoreData_ReturnsBadRequest_WhenNameIsNullOrEmpty(string? name)
    {
        //Arrange
        //Action
        var result = await _sut.StoreData("sample", "sample", "mahdddd", name, "ma", "8");
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenCreatingTheDataGroupIsFail()
    {
        //Arrange
        _storHandler.StoreDataSet("mahdddd", "8").Returns(-1);
        //Action
        var result = await _sut.StoreData("sample", "sample", "mahdddd", "name", "ma", "8");
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenEdageStorerStoreValuesReturnFalse()
    {
        //Arrange
        _storHandler.EdageStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(false);
        //Action
        var result = await _sut.StoreData("sample", "sample", "test", "mahdddd", "mahdddd", "8");
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenVertexStorerStoreValuesReturnFalse()
    {
        // Arrange
        _storHandler.VertexStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", "mamama", "mmm", "2");

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsSuccess_WhenInputAreValid()
    {
        // Arrange
        _storHandler.StoreDataSet(Arg.Any<string>(), Arg.Any<string>()).Returns(9);
        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", "a", "lll", "2");
        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }
}