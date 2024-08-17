using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Dto.UserDtos;

public class RegisterUserDto
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }

    [Required] public string FirstName { get; set; }

    [Required] public string LastName { get; set; }
    [Required] public string Email { get; set; }
}