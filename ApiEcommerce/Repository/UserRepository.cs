using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Utils;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;
    private string? secretKey;
    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }
    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return _db.Users.OrderBy(u => u.Username).ToList();
    }

    public bool isUniqueUser(string username)
    {
        return !_db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
    }

    public UserLoginResponseDto Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Username is required" };
        }

        

        var user = _db.Users.FirstOrDefault(u => u.Username == userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Bad Credentials" };
        }

        if (!SecurityAdapter.isSamePassword(userLoginDto.Password!, user.Password!))
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Bad Credentials" };
        }

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La clave secreta no puede estar vacía.");
        }   
          
        var tokenString = SecurityAdapter.GenerateToken(user, secretKey);                  
        
        return new UserLoginResponseDto()
        {
            User = new UserRegisterDto()
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Role = user.Role
            },
            token = tokenString,
            Message = "Login successful",
        };

    }

    public User Register(CreateUserDto createUserDto)
    {
        string encryptedPassword = SecurityAdapter.generatePasswordHash(createUserDto.Password!);        
        var user = new User()
        {
            Username = createUserDto.Username,
            Name = createUserDto.Name,
            Password = encryptedPassword,
            Role = createUserDto.Role
        };
        _db.Users.Add(user);
        _db.SaveChanges();
        return user;
    }


}
