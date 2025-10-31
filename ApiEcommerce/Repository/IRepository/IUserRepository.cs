using System;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{

    ICollection<User> GetUsers();

    User? GetUser(int id);

    bool isUniqueUser(string username);

    UserLoginResponseDto Login(UserLoginDto userLoginDto);

    User Register(CreateUserDto userRegisterDto); 


}
