using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;
using ApiEcommerce.Repository.IRepository;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

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

        if (string.IsNullOrEmpty(userLoginDto.Password))
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Password is required" };
        }

        var user = _db.Users.FirstOrDefault(u => u.Username == userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "User not found" };
        }

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Incorrect password" };
        }

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La clave secreta no puede estar vacía.");
        }   
          //TODO Generar el token fuera del repositorio

          //entrada secretKey ,user, expiracion     
        var key = System.Text.Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Role, user.Role??"")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var handledToken = new JwtSecurityTokenHandler();
        var token = handledToken.CreateToken(tokenDescriptor);        
          
        //TODO Generar el token fuera del repositorio
        //Regresa una cadena
        return new UserLoginResponseDto()
        {
            User = new UserRegisterDto()
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Role = user.Role
            },
            token = handledToken.WriteToken(token),
            Message = "Login successful",
        };

    }

    public User Register(CreateUserDto createUserDto)
    {
        var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
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
