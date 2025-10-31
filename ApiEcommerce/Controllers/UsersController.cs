using System;
using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace ApiEcommerce.Controllers;

[Route("api/users")]
[ApiController]
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
            user.Password = null; // Omitir la contraseña en la respuesta
            usersDto.Add(_mapper.Map<UserDto>(user));
        }
        return Ok(usersDto);
    }

    [HttpGet("{id:int}", Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(int id)
    {
        var user = _userRepository.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }
        user.Password = null; // Omitir la contraseña en la respuesta
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }


    [HttpPost("register", Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public IActionResult RegisterUser([FromBody] CreateUserDto createUserDto)
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
        var result = _userRepository.Register(createUserDto);
        result.Password = null; // Omitir la contraseña en la respuesta
        if (result == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario.");
        }
        return CreatedAtRoute("GetUser", new { id = result.Id }, result);
    }



    [HttpPost("login", Name = "LoginUser")]      
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public IActionResult LoginUser([FromBody] UserLoginDto userLoginDto )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _userRepository.Login(userLoginDto);        
        if (user == null)
        {
            return Unauthorized("Credenciales inválidas.");
        }
        return Ok(user);
    }


    
}
