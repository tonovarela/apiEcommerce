using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Dtos.User;
using ApiEcommerce.Models.Entities;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{

    ICollection<ApplicationUser> GetUsers();

    ApplicationUser? GetUser(string id);

    bool isUniqueUser(string username);

    Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);

    Task<UserDataDto> Register(CreateUserDto userRegisterDto); 


}
