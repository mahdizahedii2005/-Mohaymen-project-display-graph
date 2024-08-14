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


    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
        };

        ServiceResponse<int> response =
            await _adminService.Register(user, request.Password);

        return StatusCode((int)response.Type, response.Message);
    }

    [HttpPut("addRole")]
    public async Task<IActionResult> AddRole(AddUserRoleDto request)
    {
        ServiceResponse<string> response =
            await _adminService.AddRole(
                new User { Username = request.Username },
                new Role() { RoleType = (RoleType)Enum.Parse(typeof(RoleType), request.RoleType) }
            );

        return StatusCode((int)response.Type, response.Message);
    }

    [HttpPut("deleteRole")]
    public async Task<IActionResult> DeleteRole(DeleteUserRoleDto request)
    {
        ServiceResponse<string> response =
            await _adminService.DeleteRole(
                new User { Username = request.Username },
                new Role() { RoleType = (RoleType)Enum.Parse(typeof(RoleType), request.RoleType) }
            );

        return StatusCode((int)response.Type, response.Message);
    }


    // test
    [HttpPost("register2Test")]
    public async Task<IActionResult> Register2(UserRegisterDto request)
    {
        ServiceResponse<string> response =
            await _adminService.RegisterRoleTest(
                new User { Username = request.Username },
                request.Password
            );

        return StatusCode((int)response.Type, response.Message);
    }

    [HttpPost("register1Test")]
    public async Task<IActionResult> Register1(UserRegisterDto request)
    {
        ServiceResponse<int> response =
            await _adminService.RegisterUser(new User { Username = request.Username }, request.Password);
        return StatusCode((int)response.Type, response.Message);
    }

    [HttpGet("addRole1Test")]
    public async Task<IActionResult> AddRole1()
    {
        ServiceResponse<string> response =
            await _adminService.AddRoleTest();
        return StatusCode((int)response.Type, response.Message);
    }
}