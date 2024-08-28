using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.DataAdminService;

public class DataAdminServiceTest
{
    private readonly IStorHandler _storHandler;
    private readonly IDisplayDataService _displayDataService;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    private DataContext _dataContext;
    private readonly mohaymen_codestar_Team02.Services.DataAdminService.DataAdminService _sut;
    private readonly IEdgeService _edgeService;
    private readonly IVertexService _vertexService;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IGraphService _graphService;

    public DataAdminServiceTest()
    {
        _graphService = Substitute.For<IGraphService>();
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();
        _vertexService = Substitute.For<IVertexService>();
        _edgeService = Substitute.For<IEdgeService>();
        _storHandler = Substitute.For<IStorHandler>();
        _displayDataService = Substitute.For<IDisplayDataService>();
        _mapper = Substitute.For<IMapper>();

        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();


        _sut = new mohaymen_codestar_Team02.Services.DataAdminService.DataAdminService(_serviceProvider, _tokenService,
            _cookieService, _storHandler, _displayDataService, _edgeService, _vertexService, _mapper, _graphService);
        _storHandler.EdageStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(true);
        _storHandler.VertexStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(true);
    }

    private void FixTheReturnOfCookies(string? returnThis)
    {
        _cookieService.GetCookieValue().Returns(returnThis);
        _tokenService.GetUserNameFromToken().Returns(returnThis);
    }

    private UserRole AddUserWithRole(string userName, string roleType, long id)
    {
        using var scope = _serviceProvider.CreateScope();
        var _mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var user = new User
        {
            Salt = Array.Empty<byte>(),
            PasswordHash = Array.Empty<byte>(),
            Username = userName,
            UserId = id
        };
        var role = new Role { RoleType = roleType, RoleId = id };
        var userRole = new UserRole { UserId = user.UserId, RoleId = role.RoleId };
        _mockContext.Users.Add(user);
        _mockContext.Roles.Add(role);
        _mockContext.UserRoles.Add(userRole);
        _mockContext.SaveChanges();
        return new UserRole { Role = role, User = user };
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task StoreData_ReturnsBadRequest_WhenNameIsNullOrEmpty(string? name)
    {
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SustemAdmin", 1);
        //Arrange
        //Action
        var result = await _sut.StoreData("sample", "sample", "mahdddd", name, "ma");
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenCreatingTheDataGroupIsFail()
    {
        //Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SustemAdmin", 1);
        _storHandler.StoreDataSet("mahdddd", Arg.Any<string>()).Returns(-1);
        //Action
        var result = await _sut.StoreData("sample", "sample", "mahdddd", "name", "ma");
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenEdageStorerStoreValuesReturnFalse()
    {
        //Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SustemAdmin", 1);
        _storHandler.EdageStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(false);
        //Action
        var result = await _sut.StoreData("sample", "sample", "test", "mahdddd", "mahdddd");
        //Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsBadRequest_WhenVertexStorerStoreValuesReturnFalse()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SustemAdmin", 1);
        _storHandler.VertexStorer.StoreFileData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>()).Returns(false);

        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", "mamama", "mmm");

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task StoreData_ReturnsSuccess_WhenInputAreValid()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SustemAdmin", 1);
        _storHandler.StoreDataSet(Arg.Any<string>(), Arg.Any<string>()).Returns(9);
        // Act
        var result = await _sut.StoreData("sampleEdgeFile", "sampleVertexFile", "testData", "a", "lll");
        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public void DisplayDataSet_ShouldReturnDataSet_WhenGivenCorrectUsername()
    {
        using var scope = _serviceProvider.CreateScope();
        _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SustemAdmin", 1);
        var username = "username1";
        var datasetName1 = "Dataset1";
        var datasetName2 = "Dataset2";
        var vertexEntityName1 = "Account1";
        var EdgeEntityName1 = "Transaction1";
        var vertexEntityName2 = "Account2";
        var EdgeEntityName2 = "Transaction2";

        var GetDataGroupDtos = new List<GetDataGroupDto>()
        {
            new()
            {
                Name = datasetName1,
                CreateAt = DateTime.MaxValue,
                UpdateAt = DateTime.MaxValue,
                VertexEntity = new GetVertexEntityDto()
                {
                    Name = vertexEntityName1
                },
                EdgeEntity = new GetEdgeEntityDto()
                {
                    Name = EdgeEntityName1
                }
            },
            new()
            {
                Name = datasetName2,

                CreateAt = DateTime.MaxValue,
                UpdateAt = DateTime.MaxValue,
                VertexEntity = new GetVertexEntityDto()
                {
                    Name = vertexEntityName2
                },
                EdgeEntity = new GetEdgeEntityDto()
                {
                    Name = EdgeEntityName2
                }
            }
        };

        var expected = new ServiceResponse<List<GetDataGroupDto>>(GetDataGroupDtos, ApiResponseType.Success, "");

        var dataset1 = new DataGroup
        {
            Name = datasetName1,
            UserId = 1,
            DataGroupId = 1,
            CreateAt = DateTime.MaxValue,
            UpdateAt = DateTime.MaxValue,
            VertexEntity = new VertexEntity(vertexEntityName1, 1),
            EdgeEntity = new EdgeEntity(EdgeEntityName1, 1)
        };
        var dataset2 = new DataGroup
        {
            Name = datasetName2,
            UserId = 1,
            DataGroupId = 2,
            CreateAt = DateTime.MaxValue,
            UpdateAt = DateTime.MaxValue,
            VertexEntity = new VertexEntity(vertexEntityName2, 2),
            EdgeEntity = new EdgeEntity(EdgeEntityName2, 2)
        };

        _dataContext.DataSets.Add(dataset1);
        _dataContext.DataSets.Add(dataset2);
        _dataContext.SaveChanges();


        _mapper.Map<GetDataGroupDto>(Arg.Is<DataGroup>(dg => dg.Name == datasetName1))
            .Returns(new GetDataGroupDto
            {
                Name = datasetName1,
                CreateAt = DateTime.MaxValue,
                UpdateAt = DateTime.MaxValue,
                VertexEntity = new GetVertexEntityDto { Name = vertexEntityName1 },
                EdgeEntity = new GetEdgeEntityDto { Name = EdgeEntityName1 }
            });

        _mapper.Map<GetDataGroupDto>(Arg.Is<DataGroup>(dg => dg.Name == datasetName2))
            .Returns(new GetDataGroupDto
            {
                Name = datasetName2,
                CreateAt = DateTime.MaxValue,
                UpdateAt = DateTime.MaxValue,
                VertexEntity = new GetVertexEntityDto { Name = vertexEntityName2 },
                EdgeEntity = new GetEdgeEntityDto { Name = EdgeEntityName2 }
            });

        // Act
        var actual = _sut.DisplayDataSet();

        // Assert
        Assert.Equivalent(expected, actual);
    }

    public ServiceResponse<List<GetAttributeDto>> GetVertexAttributes(long vertexEntityId)
    {
        var att = _vertexService.GetVertexAttributes(vertexEntityId);
        return new ServiceResponse<List<GetAttributeDto>>(att, ApiResponseType.Success, "");
    }
}