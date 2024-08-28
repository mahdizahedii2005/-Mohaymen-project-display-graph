using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Dto.UserDtos;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.ProfileService;

namespace mohaymen_codestar_Team02.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword([FromQuery] ChangePasswordUserDto request)
    {
        ServiceResponse<object> response =
            await _profileService.ChangePassword(request.PreviousPassword, request.NewPassword);
        return StatusCode((int)response.Type, response);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromQuery] UpdateUserDto request)
    {
        ServiceResponse<GetUserDto?> response = await _profileService.UpdateUser(request);
        return StatusCode((int)response.Type, response);
    }
}