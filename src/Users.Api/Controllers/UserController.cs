using Microsoft.AspNetCore.Mvc;
using Users.Api.Dtos;
using Users.Api.Mappers;
using Users.Api.Models;
using Users.Api.Services;

namespace Users.Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        var usersDto = users.Select(x => x.ToUserDto());
        return Ok(usersDto);
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        var userDto = user.ToUserDto();

        return Ok(userDto);
    }

    [HttpPost("users")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
    {
        var user = new User
        {
            FullName = createUserDto.FullName
        };

        var created = await _userService.CreateAsync(user);

        if (!created)
        {
            // Implement validation
            return BadRequest();
        }

        var userDto = user.ToUserDto();

        return CreatedAtAction(nameof(GetById), new {id = userDto.Id}, userDto);
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var deleted = await _userService.DeleteByIdAsync(id);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}
