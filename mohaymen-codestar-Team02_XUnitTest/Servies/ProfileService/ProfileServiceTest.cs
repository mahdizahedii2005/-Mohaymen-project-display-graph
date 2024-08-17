using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.ProfileService
{
    public class ProfileServiceTests
    {
        private readonly mohaymen_codestar_Team02.Services.ProfileService.ProfileService _sut;
        private readonly ICookieService _cookieService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _mockContext;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public ProfileServiceTests()
        {
            _passwordService = Substitute.For<IPasswordService>();
            _cookieService = Substitute.For<ICookieService>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _tokenService = Substitute.For<ITokenService>();

            // تنظیم Mapper
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<UpdateUserDto, User>(); });
            _mapper = config.CreateMapper();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _mockContext = new DataContext(options);

            _sut = new mohaymen_codestar_Team02.Services.ProfileService.ProfileService(
                _httpContextAccessor, _mockContext, _cookieService, _passwordService, _tokenService, _mapper);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnUnauthorized_WhenCookieIsEmpty()
        {
            // Arrange
            _cookieService.GetCookieValue().Returns((string)null);

            // Act
            var result = await _sut.ChangePassword("oldPassword", "newPassword");

            // Assert
            Assert.Equal(ApiResponseType.Unauthorized, result.Type);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenUserNotFound()
        {
            // Arrange
            _cookieService.GetCookieValue().Returns("validToken");
            _tokenService.GetUserNameFromToken().Returns("nonexistentUser");

            // Act
            var result = await _sut.ChangePassword("oldPassword", "newPassword");

            // Assert
            Assert.Equal(ApiResponseType.BadRequest, result.Type);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenWrongPassword()
        {
            // Arrange
            var user = AddUserToDatabase("existingUser", "oldPassword");
            _cookieService.GetCookieValue().Returns("validToken");
            _tokenService.GetUserNameFromToken().Returns("existingUser");
            _passwordService.VerifyPasswordHash("newPassword", user.PasswordHash, user.Salt).Returns(false);

            // Act
            var result = await _sut.ChangePassword("oldPassword", "newPassword");

            // Assert
            Assert.Equal(ApiResponseType.BadRequest, result.Type);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnSuccess_WhenPasswordIsChanged()
        {
            // Arrange
            var user = AddUserToDatabase("existingUser", "oldPassword");
            _cookieService.GetCookieValue().Returns("validToken");
            _tokenService.GetUserNameFromToken().Returns("existingUser");
            _passwordService.VerifyPasswordHash(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<byte[]>()).Returns(true);
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
            var result = await _sut.ChangePassword("oldPassword", "newPassword");

            // Assert
            Assert.Equal(ApiResponseType.Success, result.Type);

            // Verify that the user's password was update
        }

        [Fact]
        public void Logout_ShouldClearLoginCookie_WhenCookieExists()
        {
            // Arrange
            var context = Substitute.For<HttpContext>();
            var responseCookies = Substitute.For<IResponseCookies>();
            context.Response.Cookies.Returns(responseCookies);

            var requestCookies = Substitute.For<IRequestCookieCollection>();
            requestCookies.ContainsKey("login").Returns(true); // Ensure the cookie "login" exists

            context.Request.Cookies.Returns(requestCookies);
            _httpContextAccessor.HttpContext.Returns(context);

            // Act
            var result = _sut.Logout();

            // Assert
            Assert.Equal(ApiResponseType.Success, result.Type);
            responseCookies.Received(1)
                .Append("login", "", Arg.Is<CookieOptions>(options => options.Expires < DateTime.Now));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUnauthorized_WhenCookieIsEmpty()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
                { FirstName = "NewFirstName", LastName = "NewLastName", Email = "newemail@example.com" };
            _cookieService.GetCookieValue().Returns(string.Empty);

            // Act
            var result = await _sut.UpdateUser(updateUserDto);

            // Assert
            Assert.Equal(ApiResponseType.Unauthorized, result.Type);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenUserNotFound()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
                { FirstName = "NewFirstName", LastName = "NewLastName", Email = "newemail@example.com" };
            _cookieService.GetCookieValue().Returns("validToken");
            _tokenService.GetUserNameFromToken().Returns("nonexistentUser");

            // Act
            var result = await _sut.UpdateUser(updateUserDto);

            // Assert
            Assert.Equal(ApiResponseType.BadRequest, result.Type);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnSuccess_WhenUserIsUpdated()
        {
            // Arrange
            var user = AddUserToDatabase("existingUser", "password");
            _cookieService.GetCookieValue().Returns("validToken");
            _tokenService.GetUserNameFromToken().Returns("existingUser");

            var updateUserDto = new UpdateUserDto
                { FirstName = "NewFirstName", LastName = "NewLastName", Email = "newemail@example.com" };

            // Act
            var result = await _sut.UpdateUser(updateUserDto);

            // Assert
            Assert.Equal(ApiResponseType.Success, result.Type);
        }

        private User AddUserToDatabase(string username, string password)
        {
            var user = new User
            {
                Username = username,
                Salt = new byte[128],
                PasswordHash = new byte[64],
            };

            using (var hmac = new System.Security.Cryptography.HMACSHA512(user.Salt))
            {
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            _mockContext.Users.Add(user);
            _mockContext.SaveChanges();
            return user;
        }
    }
}