using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace ApiEcommerce.Controllers;

[Route("api/v{version:apiVersion}/users")]
[ApiController]
[ApiVersionNeutral]   
[EnableCors(PolicyNames.AllowSpecificOrigins)]
[Authorize]
public class UsersController : ControllerBase
{

    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }


    [HttpGet]    
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetUsers();
        List<UserDto> usersDto = new List<UserDto>();
        foreach (var user in users)
        {
            usersDto.Add(_mapper.Map<UserDto>(user));
        }
        return Ok(usersDto);
    }

    [HttpGet("{id}", Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(string id)
    {
        var user =  _userRepository.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }


    [HttpPost("register", Name = "RegisterUser")]
    //[ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (string.IsNullOrWhiteSpace(createUserDto.Username))
        {
            return BadRequest("El campo username no puede estar vacío o contener solo espacios en blanco.");
        }
        if (!_userRepository.isUniqueUser(createUserDto.Username))
        {
            return BadRequest("El nombre de usuario ya existe.");
        }
        var result =await  _userRepository.Register(createUserDto);
        //result.Password = null; // Omitir la contraseña en la respuesta
        if (result == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario.");
        }
                
        return CreatedAtRoute("GetUser", new { id = result.Id! }, result);
    }



    [HttpPost("login", Name = "LoginUser")]      
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userRepository.Login(userLoginDto);        
        if (user == null)
        {
            return Unauthorized("Credenciales inválidas.");
        }
        return Ok(user);
    }


    
}
