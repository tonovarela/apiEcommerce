
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiEcommerce.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Utils;

public static  class SecurityAdapter
{

    public static string GenerateToken(User user, string secretKey)
    {

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
        return handledToken.WriteToken(token);
    }

    public static string generatePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    public static bool isSamePassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
}



