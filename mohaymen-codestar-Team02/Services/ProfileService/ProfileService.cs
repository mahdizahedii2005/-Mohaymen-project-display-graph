using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public class ProfileService : IProfileService
{
    private readonly DataContext _context;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public ProfileService(DataContext context, ICookieService cookieService,
        IPasswordService passwordService, ITokenService tokenService, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _cookieService = cookieService;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetUserDto?>> ChangePassword(string previousPassword, string newPassword)
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);
        }

        var username = _tokenService.GetUserNameFromToken();
        var user = await GetUser(username);

        if (user is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!_passwordService.VerifyPasswordHash(newPassword, user.PasswordHash, user.Salt))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.WrongPasswordMessage);

        _passwordService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var userDto = _mapper.Map<GetUserDto>(user);
        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success,
            Resources.PasswordChangedSuccessfulyMessage);
    }

    public async Task<ServiceResponse<GetUserDto?>> UpdateUser(UpdateUserDto updateUserDto)
    {
        var newUser = _mapper.Map<UpdateUserDto>(updateUserDto);

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);
        }

        var username = _tokenService.GetUserNameFromToken();
        var user = await GetUser(username);
        if (user is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        user.FirstName = newUser.FirstName;
        user.LastName = newUser.LastName;
        user.Email = newUser.Email;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var userDto = _mapper.Map<GetUserDto>(user);
        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success,
            Resources.ProfileInfoUpdateSuccessfulyMessage);
    }

    private Task<User?> GetUser(string? username) =>
        _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
}