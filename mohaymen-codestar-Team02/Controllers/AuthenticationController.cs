using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.UserDtos;
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
        var response = await _authenticationService.Login(request.Username, request.Password);
        return StatusCode((int)response.Type, response.Message);
    }
}