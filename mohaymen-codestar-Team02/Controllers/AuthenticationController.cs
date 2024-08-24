using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.Permission;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Dto.UserDtos;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Authenticatoin;

namespace mohaymen_codestar_Team02.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto request)
    {
        ServiceResponse<GetUserDto?> response = await _authenticationService.Login(request.Username, request.Password);
        return StatusCode((int)response.Type, response);
    }

    [HttpPost("logout")]
    //[ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        ServiceResponse<string?> response = _authenticationService.Logout();
        return StatusCode((int)response.Type, response);
    }

    [HttpGet("permission")]
    public async Task<IActionResult> GetPermission()
    {
        ServiceResponse<GetPermissionDto> response = await _authenticationService.GetPermission();
        return StatusCode((int)response.Type, response);
    }
}