using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public class AuthenticationService : IAuthenticationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public AuthenticationService(IServiceProvider serviceProvider, ICookieService cookieService,
        ITokenService tokenService,
        IPasswordService passwordService, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetUserDto?>> Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.InvalidInpute);

        var user = await GetUser(username);

        if (user is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!_passwordService.VerifyPasswordHash(password, user.PasswordHash, user.Salt))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.WrongPasswordMessage);

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        _cookieService.CreateCookie(_tokenService.CreateToken(claims));

        var userDto = _mapper.Map<GetUserDto>(user);

        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success, Resources.LoginSuccessfulMessage);
    }

    public ServiceResponse<string?> Logout()
    {
        if (_cookieService.GetCookieValue() != null) _cookieService.GetExpiredCookie();

        return new ServiceResponse<string?>(null, ApiResponseType.Success,
            Resources.LogoutSuccessfuly);
    }

    private async Task<User?> GetUser(string username)
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();

        return await _context.Users.FirstOrDefaultAsync(x =>
            x.Username != null && x.Username.ToLower().Equals(username.ToLower()));
    }
}