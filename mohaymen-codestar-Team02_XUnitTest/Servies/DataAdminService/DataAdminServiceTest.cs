using System.Dynamic;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using NSubstitute;

namespace mohaymen_codestar_Team02.Services.DataAdminService;

public class DataAdminServiceTest
{
    private readonly IStorHandler _storHandler;
    private readonly DataAdminService _sut;

    public DataAdminServiceTest()
    {
        _storHandler = Substitute.For<IStorHandler>();
        var modelHandler = Substitute.For<IModelHandler>();
        _sut = new DataAdminService(modelHandler, _storHandler);
        _storHandler.EdageStorer.StoreValues(Arg.Any<IEnumerable<ExpandoObject>>()).Returns(true);
        _storHandler.VertexStorer.StoreValues(Arg.Any<IEnumerable<ExpandoObject>>()).Returns(true);
        _storHandler.EdageStorer.StoreAttribute(Arg.Any<IEnumerable<string>>()).Returns(true);
        _storHandler.VertexStorer.StoreAttribute(Arg.Any<IEnumerable<string>>()).Returns(true);
        _storHandler.EdageStorer.StoreEntity(Arg.Any<Type>()).Returns(true);
        _storHandler.VertexStorer.StoreEntity(Arg.Any<Type>()).Returns(true);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenUpdateTimeIsNull()
    {
        //Arrange
        //Action
        var result = _sut.StoreData("sample", "sample", "test", null, new DateTime());
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenCreatTimeIsNull()
    {
        //Arrange
        //Action
        var result = _sut.StoreData("sample", "sample", "test", new DateTime(), null);
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task StoreData_ReturnsBadRequest_WhenNameIsNullOrEmpty(string? name)
    {
        //Arrange
        //Action
        var result = _sut.StoreData("sample", "sample", name, new DateTime(), new DateTime());
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenEdageStorerStoreValuesReturnFalse()
    {
        //Arrange
        _storHandler.EdageStorer.StoreValues(Arg.Any<IEnumerable<ExpandoObject>>()).Returns(false);
        //Action
        var result = _sut.StoreData("sample", "sample", "test", new DateTime(), new DateTime());
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenInputIsInvalid()
    {
        // Arrange
        // (No specific arrangement needed here for invalid inputs)

        // Act
        var result = await _sut.StoreData(null, "vertexFile", null, null, null);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenEdageStorerStoreEntityReturnFalse()
    {
        // Arrange
        _storHandler.EdageStorer.StoreEntity(Arg.Any<Type>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", DateTime.Now, DateTime.Now);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenVertexStorerStoreEntityReturnFalse()
    {
        // Arrange
        _storHandler.VertexStorer.StoreEntity(Arg.Any<Type>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", DateTime.Now, DateTime.Now);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenEdageStorerStoreAttributeReturnFalse()
    {
        // Arrange
        _storHandler.EdageStorer.StoreAttribute(Arg.Any<IEnumerable<string>>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", DateTime.Now, DateTime.Now);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenVertexStorerStoreAttributeReturnFalse()
    {
        // Arrange
        _storHandler.VertexStorer.StoreAttribute(Arg.Any<IEnumerable<string>>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", DateTime.Now, DateTime.Now);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenVertexStorerStoreValuesReturnFalse()
    {
        // Arrange
        _storHandler.VertexStorer.StoreValues(Arg.Any<IEnumerable<ExpandoObject>>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", DateTime.Now,
            DateTime.Now);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }
    [Fact]
    public async Task StoreData_ReturnsSuccses_WhenInputAreValid()
    {
        // Arrange
        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", DateTime.Now,
            DateTime.Now);
        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

}