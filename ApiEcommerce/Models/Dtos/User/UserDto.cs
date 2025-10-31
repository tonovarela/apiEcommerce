

using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class UserDto
{

   
    
    public string? Name { get; set; } 
    
    public string? Username { get; set; }   
    
    public string? Password { get; set; }    
        
    public string? Role { get; set; }
}
