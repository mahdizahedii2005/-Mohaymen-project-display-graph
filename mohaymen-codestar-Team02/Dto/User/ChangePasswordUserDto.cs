using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Dto.UserDtos;

public class ChangePasswordUserDto
{
    [Required] public string PreviousPassword { get; set; }
    [Required] public string NewPassword { get; set; }
}