using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.CookieService;

public class CookieServiceTestTests
{
    private readonly mohaymen_codestar_Team02.Services.CookieService.CookieService _sut;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpContext _httpContext;
    private readonly IResponseCookies _responseCookies;
    private readonly IRequestCookieCollection _requestCookies;

    public CookieServiceTestTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _httpContext = Substitute.For<HttpContext>();
        _responseCookies = Substitute.For<IResponseCookies>();
        _requestCookies = Substitute.For<IRequestCookieCollection>();

        _httpContextAccessor.HttpContext.Returns(_httpContext);
        _sut = new mohaymen_codestar_Team02.Services.CookieService.CookieService(_httpContextAccessor);
    }

    [Fact]
    public void CreateCookie_ShouldAppendCookie_WithCorrectOptions()
    {
        // Arrange
        string testData = "test_cookie_data";
        _httpContext.Response.Cookies.Returns(_responseCookies);

        // Act
        _sut.CreateCookie(testData);

        // Assert
        _responseCookies.Received(1).Append("login", testData, Arg.Is<CookieOptions>(options =>
            options.HttpOnly == true &&
            options.Secure == true &&
            options.Expires.Value.Date == DateTime.Now.AddDays(1).Date));
    }

    [Fact]
    public void GetCookieValue_ShouldReturnCorrectValue_WhenCookieExists()
    {
        // Arrange
        string expectedCookieValue = "test_cookie_value";
        _requestCookies["login"].Returns(expectedCookieValue);
        _httpContext.Request.Cookies.Returns(_requestCookies);

        // Act
        var result = _sut.GetCookieValue();

        // Assert
        Assert.Equal(expectedCookieValue, result);
    }

    [Fact]
    public void GetCookieValue_ShouldReturnNull_WhenCookieDoesNotExist()
    {
        // Arrange
        _requestCookies["login"].Returns((string?)null);
        _httpContext.Request.Cookies.Returns(_requestCookies);

        // Act
        var result = _sut.GetCookieValue();

        // Assert
        Assert.Null(result);
    }
}