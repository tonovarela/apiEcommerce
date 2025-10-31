using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class UserProfile : Profile
{

    public UserProfile()
    {   
        CreateMap<User, UserDto>().ReverseMap();     
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UserLoginResponseDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserRegisterDto>().ReverseMap();
        
    }
}
