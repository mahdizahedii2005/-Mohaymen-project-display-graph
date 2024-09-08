using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Dto.UserDtos;

public class LoginUserDto
{
    [Required] public string Username { get; init; }
    [Required] public string Password { get; init; }
}