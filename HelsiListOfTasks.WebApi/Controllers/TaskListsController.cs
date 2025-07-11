using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelsiListOfTasks.WebApi.Controllers;

[ApiController]
[Route("tasklists")]
public class TaskListsController(ITaskListService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskList model)
    {
        var result = await service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = result.Id, userId = result.OwnerId }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] int userId)
    {
        var result = await service.GetByIdAsync(id, userId);
        if (result is null) return NotFound();
        return Ok(result);
    }
}