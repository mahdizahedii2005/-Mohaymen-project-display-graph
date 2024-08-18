namespace mohaymen_codestar_Team02.Services.CookieService;

public interface ICookieService
{
    void CreateCookie(string data);
    string? GetCookieValue();
    void GetExpiredCookie();
}