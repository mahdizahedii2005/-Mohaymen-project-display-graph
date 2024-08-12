using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Administration;

namespace mohaymen_codestar_Team02.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        ServiceResponse<int> response =
            await _adminService.Register(new User {Username = request.Username}, request.Password);
        return StatusCode((int)response.Type, response.Message);
    }
    
    [HttpPost("changeRole")]
    public async Task<IActionResult> ChangeRole(UserChangeAccessLevelDto request)
    {
        ServiceResponse<string> response =
            await _adminService.ChangeRole(new User {Username = request.Username}, request.newRole);
        return StatusCode((int)response.Type, response.Message);
    }
}