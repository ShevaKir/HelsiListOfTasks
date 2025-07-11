using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HelsiListOfTasks.WebApi.Controllers;

[ApiController]
[Route("tasklists")]
public class TaskListsController(ITaskListService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskListRequest request,
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        if (userId is null)
            return BadRequest("Missing X-User-Id header");

        var taskList = new TaskList
        {
            Title = request.Title,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var result = await service.CreateAsync(taskList);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] string userId)
    {
        var result = await service.GetByIdAsync(id, userId);
        if (result is null) return NotFound();
        return Ok(result);
    }
}