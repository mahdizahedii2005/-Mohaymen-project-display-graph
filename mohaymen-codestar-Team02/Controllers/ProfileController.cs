using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.User;
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
        ServiceResponse<User> response =
            await _profileService.ChangePassword(request.PreviousPassword, request.NewPassword);
        return StatusCode((int)response.Type, response.Message);
    }

    [HttpPost("logout")]
    //[ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        ServiceResponse<User> response = _profileService.Logout();
        return StatusCode((int)response.Type, response.Message);
    }

    [HttpPost("UpdateUser")]
    public async Task<IActionResult> UpdateUser(UpdateUserDto request)
    {
        ServiceResponse<User> response = await _profileService.UpdateUser(request);
        return StatusCode((int)response.Type, response.Message);
    }
}