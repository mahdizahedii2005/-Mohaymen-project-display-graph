using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.Role;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Dto.UserDtos;
using mohaymen_codestar_Team02.Dto.UserRole;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Administration;

namespace mohaymen_codestar_Team02.Controllers;

[ApiController]
[Authorize(Roles = nameof(RoleType.SystemAdmin))]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        ServiceResponse<List<GetUserDto>?> response =
            await _adminService.GetAllUsers();
        return StatusCode((int)response.Type, response);
    }

    [HttpGet("users/{username}")]
    public async Task<IActionResult> GetSingleUser(string? username)
    {
        ServiceResponse<GetUserDto?> response =
            await _adminService.GetUserByUsername(username);
        return StatusCode((int)response.Type, response);
    }

    [HttpPost("users")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        ServiceResponse<GetUserDto?> response =
            await _adminService.Register(user, request.Password);

        return StatusCode((int)response.Type, response);
    }

    [HttpDelete("users/{username}")]
    public async Task<IActionResult> Delete(string username)
    {
        var user = new User
        {
            Username = username
        };

        ServiceResponse<GetUserDto?> response =
            await _adminService.DeleteUser(user);

        return StatusCode((int)response.Type, response);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        ServiceResponse<List<GetRoleDto>> response =
            await _adminService.GetAllRoles();
        return StatusCode((int)response.Type, response);
    }

    [HttpPut("users/{username}/roles")]
    public async Task<IActionResult> AddRole([FromBody] AddUserRoleDto request, string username)
    {
        ServiceResponse<GetUserDto?> response =
            await _adminService.AddRole(
                new User { Username = username },
                new Role() { RoleType = request.RoleType }
            );

        return StatusCode((int)response.Type, response);
    }

    [HttpDelete("users/{username}/roles")]
    public async Task<IActionResult> DeleteRole([FromBody] DeleteUserRoleDto request, string username)
    {
        ServiceResponse<GetUserDto?> response =
            await _adminService.DeleteRole(
                new User { Username = username },
                new Role() { RoleType = request.RoleType }
            );

        return StatusCode((int)response.Type, response);
    }
}