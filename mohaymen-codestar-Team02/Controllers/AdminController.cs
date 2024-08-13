using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.UserDtos;
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
    
    [HttpPost("register2")]
    public async Task<IActionResult> Register2(UserRegisterDto request)
    {
        ServiceResponse<string> response =
            await _adminService.Regiser2(new User {Username = request.Username}, request.Password);
        return StatusCode((int)response.Type, response.Message);
    }
    
    [HttpPost("register1")]
    public async Task<IActionResult> Register1(UserRegisterDto request)
    {
        ServiceResponse<int> response =
            await _adminService.Register1(new User {Username = request.Username}, request.Password);
        return StatusCode((int)response.Type, response.Message);
    }
    
    [HttpGet("addRole1")]
    public async Task<IActionResult> AddRole1()
    {
        ServiceResponse<string> response =
            await _adminService.AddRole1();
        return StatusCode((int)response.Type, response.Message);
    }
    
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        ServiceResponse<int> response =
            await _adminService.Register(new User {Username = request.Username}, request.Password);
        return StatusCode((int)response.Type, response.Message);
    }
    
    [HttpPut("addRole")]
    public async Task<IActionResult> AddRole(UserAddRoleDto request)
    {
        ServiceResponse<string> response =
            await _adminService.AddRole(new User {Username = request.Username}, new Role() { RoleId = request.RoleId});
        return StatusCode((int)response.Type, response.Message);
    }
    
    [HttpPut("deleteRole")]
    public async Task<IActionResult> DeleteRole(UserDeleteRoleDto request)
    {
        ServiceResponse<string> response =
            await _adminService.DeleteRole(new User {Username = request.Username}, new Role() { RoleId = request.RoleId});
        return StatusCode((int)response.Type, response.Message);
    }
}