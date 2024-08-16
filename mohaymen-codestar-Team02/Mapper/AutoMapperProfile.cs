using AutoMapper;
using mohaymen_codestar_Team02.Dto.Role;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Dto.UserDtos;
using mohaymen_codestar_Team02.Dto.UserRole;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, GetUserDto>()
            .ForMember(dto => dto.Roles, c =>
                c.MapFrom(u => u.UserRoles.Select(ur => ur.Role)));
        CreateMap<Role, GetRoleDto>();
        CreateMap<User, RegisterUserDto>();
    }
}