
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Dtos.User;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;
    private string? secretKey;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;
    protected readonly IMapper _mapper;
    public UserRepository(ApplicationDbContext db,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper
    )
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }
    public ApplicationUser? GetUser(string id)
    {
        return _db.ApplicationUsers.FirstOrDefault(u => u.Id.Equals(id));
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return _db.ApplicationUsers.OrderBy(u => u.UserName).ToList();
    }

    public bool isUniqueUser(string username)
    {
        return !_db.ApplicationUsers.Any(u => u.UserName!.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Username is required" };
        }        
        var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName!.ToLower().Trim()== userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Bad Credentials" };
        }
        bool isValidPassword = await _userManager.CheckPasswordAsync(user, userLoginDto.Password!);
        //if (!SecurityAdapter.isSamePassword(userLoginDto.Password!, user.PasswordHash!))
        if (!isValidPassword)
        {
            return new UserLoginResponseDto() { User = null, token = null, Message = "Bad Credentials" };
        }
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La clave secreta no puede estar vacía.");
        }   
        var roles = await _userManager.GetRolesAsync(user);  
        var tokenString = SecurityAdapter.GenerateToken(user, secretKey, roles.ToArray());                          
        return new UserLoginResponseDto()
        {
            User = _mapper.Map<UserDataDto>(user),
            token = tokenString,
            Message = "Login successful",
        };

    }

    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {

        if (string.IsNullOrEmpty(createUserDto.Username) ||
                 string.IsNullOrEmpty(createUserDto.Password))
        {
            throw new InvalidOperationException("Username and Password are required");
        }

        var user = new ApplicationUser()
        {
            UserName = createUserDto.Username,
            Name = createUserDto.Name,
            NormalizedEmail = createUserDto.Username.ToUpper(),
            Email = createUserDto.Username,
        };
        Console.WriteLine("Creating user...");
        try
        {
        var result = await _userManager.CreateAsync(user, createUserDto.Password!);        
        if (!result.Succeeded)
        {

             Console.WriteLine("User creation failed:");
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            //Console.WriteLine(errors);
            throw new InvalidOperationException($"User creation failed: {errors}");
        }
        var userRole = createUserDto.Role ?? "User";

        if (!await _roleManager.RoleExistsAsync(userRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(userRole));
        }
        await _userManager.AddToRoleAsync(user, userRole);

        var createdUser = _mapper.Map<UserDataDto>(user);
        return createdUser;            
        }catch(Exception ex)
        {
            Console.WriteLine($"Exception during user creation: {ex.Message}");
            throw new InvalidOperationException("Error occurred during user registration.");
        }
        
    }

   
}
