using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HelsiListOfTasks.WebApi.Controllers;

[ApiController]
[Route("task-lists")]
public class TaskListsController(ITaskListService taskListService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskListRequest request,
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

        var result = await taskListService.CreateAsync(taskList);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] string userId)
    {
        var result = await taskListService.GetByIdAsync(id, userId);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetForUser([FromHeader(Name = "X-User-Id")] string? userId)
    {
        if (userId is null)
            return BadRequest("Missing X-User-Id header");

        var lists = await taskListService.GetForUserAsync(userId);
        return Ok(lists);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TaskListRequest request,
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        if (userId is null)
            return BadRequest("Missing X-User-Id header");

        var updated = new TaskList
        {
            Id = id,
            Title = request.Title,
            OwnerId = userId
        };

        var success = await taskListService.UpdateAsync(updated, userId);
        return success ? Ok() : Forbid();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, 
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        if (userId is null)
            return BadRequest("Missing X-User-Id header");
        
        var success = await taskListService.DeleteAsync(id, userId);
        return success ? NoContent() : NotFound();
    }
}