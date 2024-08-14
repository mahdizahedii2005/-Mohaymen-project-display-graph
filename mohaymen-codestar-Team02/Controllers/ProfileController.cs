using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.UserDtos;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ProfileService;

namespace mohaymen_codestar_Team02.Controllers;

public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    
    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    
    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordUserDto request)
    {
        ServiceResponse<string> response = await _profileService.ChangePassword(request.Username, request.NewPassword);
        return StatusCode((int)response.Type, response.Message);
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        ServiceResponse<string> response = await _profileService.Logout();
        return StatusCode((int)response.Type, response.Message);
    }
}