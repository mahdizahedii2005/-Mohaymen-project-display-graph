namespace mohaymen_codestar_Team02.Services.CookieService;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void CreateCookie(string data) 
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(1)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("login", data, cookieOptions);
    }
    
    public string? GetCookieValue()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies["login"];
    }
}