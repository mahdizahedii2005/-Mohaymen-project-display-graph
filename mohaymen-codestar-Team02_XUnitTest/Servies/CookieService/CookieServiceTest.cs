using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.CookieService
{
    public class CookieServiceTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly mohaymen_codestar_Team02.Services.CookieService.CookieService _sut;

        public CookieServiceTests()
        {
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _sut = new mohaymen_codestar_Team02.Services.CookieService.CookieService(_httpContextAccessor);
        }

        [Fact]
        public void CreateCookie_ShouldAddCookieToResponse_WhenCreateCookieIsCalled()
        {
            // Arrange
            var responseCookies = Substitute.For<IResponseCookies>();
            var httpResponse = Substitute.For<HttpResponse>();
            httpResponse.Cookies.Returns(responseCookies);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Response.Returns(httpResponse);

            _httpContextAccessor.HttpContext.Returns(httpContext);
            string cookieValue = "test_cookie_value";

            // Act
            _sut.CreateCookie(cookieValue);

            // Assert
            responseCookies.Received(1).Append(
                "login",
                cookieValue,
                Arg.Is<CookieOptions>(options =>
                    options.HttpOnly && options.Secure && options.SameSite == SameSiteMode.Strict)
            );
        }

        [Fact]
        public void GetCookieValue_ShouldReturnCookieValue_WhenCookieExists()
        {
            // Arrange
            var requestCookies = Substitute.For<IRequestCookieCollection>();
            requestCookies["login"].Returns("test_cookie_value");

            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Cookies.Returns(requestCookies);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Returns(httpRequest);

            _httpContextAccessor.HttpContext.Returns(httpContext);

            // Act
            var result = _sut.GetCookieValue();

            // Assert
            Assert.Equal("test_cookie_value", result);
        }

        [Fact]
        public void GetCookieValue_ShouldReturnNull_WhenCookieDoesNotExist()
        {
            // Arrange
            var requestCookies = Substitute.For<IRequestCookieCollection>();
            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Cookies.Returns(requestCookies);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Returns(httpRequest);

            _httpContextAccessor.HttpContext.Returns(httpContext);

            // Act
            var result = _sut.GetCookieValue();

            // Assert
            Assert.Equal(string.Empty, result);
        }
        
        [Fact]
        public void GetExpiredCookie_ShouldReturnExpiredCookie_WhenGetExpiredCookieIsCalled()
        {
            // Arrange
            var responseCookies = Substitute.For<IResponseCookies>();
            var httpResponse = Substitute.For<HttpResponse>();
            httpResponse.Cookies.Returns(responseCookies);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Response.Returns(httpResponse);

            _httpContextAccessor.HttpContext.Returns(httpContext);

            // Act
            _sut.GetExpiredCookie();

            // Assert
            responseCookies.Received(1).Append(
                "login",
                "",
                Arg.Is<CookieOptions>(options =>
                    options.HttpOnly &&
                    options.Secure &&
                    options.Expires.HasValue &&
                    options.Expires.Value < DateTime.Now)
            );
        }
    }
}