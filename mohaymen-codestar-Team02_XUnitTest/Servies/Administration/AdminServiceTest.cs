using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;
using AutoMapper;
using mohaymen_codestar_Team02.Dto.Role;
using mohaymen_codestar_Team02.Dto.User;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.Administration;

public class AdminServiceTest
{
    private readonly AdminService _sut;
    private readonly ICookieService _cookieService;
    private readonly DataContext _mockContext;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public AdminServiceTest()
    {
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();
        _passwordService = Substitute.For<IPasswordService>();
        _mapper = Substitute.For<IMapper>();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: new Random().NextInt64() + Guid.NewGuid().ToString())
            .Options;
        _mockContext = new DataContext(options);
        _sut = new AdminService(_mockContext, _cookieService, _tokenService, _passwordService,
            _mapper); // پاس دادن Mapper به کلاس AdminService
    }
    [Fact]
    public async Task Register_ShouldReturnUnauthorized_WhenTokenIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.Register(null, "password");

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.Register(null, "password");

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", "Analyst", 1);

        // Act
        var result = await _sut.Register(new User(), "password");

        // Assert
        Assert.Equal(ApiResponseType.Forbidden, result.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUserAlreadyExists()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var existingUser = AddUserWithRole("existingUser", "Analyst", 2);

        // Act
        var result = await _sut.Register(existingUser.User, "password");

        // Assert
        Assert.Equal(ApiResponseType.Conflict, result.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenUserIsSuccessfullyRegistered()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        var newUser = AddUserWithRole("admin", "SystemAdmin", 1);
        byte[] fakePasswordHash = new byte[] { 1, 2, 3, 4 };
        byte[] fakePasswordSalt = new byte[] { 5, 6, 7, 8 };

        _passwordService
            .When(x => x.CreatePasswordHash(Arg.Any<string>(), out Arg.Any<byte[]>(), out Arg.Any<byte[]>()))
            .Do(x =>
            {
                x[1] = fakePasswordHash;
                x[2] = fakePasswordSalt;
            });
        // Act
        var result = await _sut.Register(
            new User(){UserId = 8,Username = "mamad"}, "password");

        // Assert
        Assert.Equal(ApiResponseType.Created, result.Type);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnSuccess_WhenUserIsFound()
    {
        // Arrange
        var user = AddUserWithRole("testUser", "SystemAdmin", 1);

        _mapper.Map<GetUserDto>(Arg.Any<User>()).Returns(new GetUserDto());

        // Act
        var result = await _sut.GetUserByUsername("testUser");

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _sut.GetUserByUsername("nonExistentUser");

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsersSuccessfully()
    {
        // Arrange
        AddUserWithRole("admin1", "SystemAdmin", 1);
        AddUserWithRole("admin2", "SystemAdmin", 2);

        _mapper.Map<GetUserDto>(Arg.Any<User>()).Returns(new GetUserDto());

        // Act
        var result = await _sut.GetAllUsers();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetAllRoles_ShouldReturnAllRolesSuccessfully()
    {
        // Arrange
        _mockContext.Roles.Add(new Role { RoleId = 1, RoleType = "SystemAdmin" });
        _mockContext.Roles.Add(new Role { RoleId = 2, RoleType = "Analyst" });
        await _mockContext.SaveChangesAsync();

        _mapper.Map<GetRoleDto>(Arg.Any<Role>()).Returns(new GetRoleDto());

        // Act
        var result = await _sut.GetAllRoles();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.Equal(2, result.Data.Count);
    }

    private void FixTheReturnOfCookies(string returnThis)
    {
        _cookieService.GetCookieValue().Returns(returnThis);
        _tokenService.GetUserNameFromToken().Returns(returnThis);
    }

    private UserRole AddUserWithRole(string userName, string roleType, long id)
    {
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

    [Fact]
    public async Task AddRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.AddRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.AddRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnForbidden_WhenCommenderIsNotSystemAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", "Analyst", 1);

        // Act
        var result = await _sut.AddRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Forbidden, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnSuccess_WhenCommenderIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);
        var role = AddUserWithRole("fakeUser", "Analyst", 3);

        // Act
        var result = await _sut.AddRole(user.User, role.Role);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.DeleteRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.DeleteRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnForbidden_WhenCommenderIsNotSystemAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", "Analyst", 1);

        // Act
        var result = await _sut.DeleteRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Forbidden, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenUserRoleNotExists()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        AddUserWithRole("lol", "SystemAdmin", 44);

        // Act
        var result = await _sut.DeleteRole(new User { Username = "lol" }, new Role() { RoleType = "invalidRole" });

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenUserRoleExistsAndUserDontHaveRole()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);
        var role = AddUserWithRole("targetRole", "Analyst", 30);

        // Act
        var result = await _sut.DeleteRole(user.User, role.Role);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnSuccess_WhenUserRoleExistsAndUserIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);

        // Act
        var result = await _sut.DeleteRole(user.User, user.Role);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldRoleBeNullIfLoockingForIt_WhenUserRoleExistsAndUserIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);

        // Act
        await _sut.DeleteRole(user.User, user.Role);

        // Assert
        var result = _mockContext.UserRoles.SingleOrDefault(ur =>
            ur.UserId == user.User.UserId && ur.RoleId == user.Role.RoleId);
        Assert.Null(result);
    }
}