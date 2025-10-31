using System;

namespace ApiEcommerce.Models.Dtos;

public class UserRegisterDto
{

    public int? Id { get; set; }
    public  string? Name { get; set; }     
    public required string Username { get; set; }       
    public  string Password { get; set; }  =string.Empty;
    public string? Role { get; set; }
}
