using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HelsiListOfTasks.WebApi.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserService userService) : ControllerBase
{
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = new User()
        {
            Name = request.Name
        };

        await userService.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is not null ? Ok(user) : NotFound();
    }
}