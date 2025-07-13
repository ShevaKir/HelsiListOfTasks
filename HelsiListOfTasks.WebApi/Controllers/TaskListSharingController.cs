using HelsiListOfTasks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelsiListOfTasks.WebApi.Controllers;

[ApiController]
[Route("task-lists/{taskListId}/sharing")]
public class TaskListSharingController(ITaskListSharingService sharingService) : ControllerBase
{
    [HttpPost("{targetUserId}")]
    public async Task<IActionResult> Share(string taskListId, string targetUserId,
        [FromHeader(Name = "X-User-Id")] string? requesterUserId)
    {
        if (requesterUserId is null)
            return BadRequest("Missing X-User-Id header");

        var result = await sharingService.AddShareAsync(taskListId, targetUserId, requesterUserId);
        return result ? Ok() : Forbid();
    }

    [HttpDelete("{targetUserId}")]
    public async Task<IActionResult> CancelSharing(string taskListId, string targetUserId,
        [FromHeader(Name = "X-User-Id")] string? requesterUserId)
    {
        if (requesterUserId is null)
            return BadRequest("Missing X-User-Id header");

        var result = await sharingService.RemoveShareAsync(taskListId, targetUserId, requesterUserId);
        return result ? Ok() : Forbid();
    }


    [HttpGet]
    public async Task<IActionResult> GetSharedUsers(string taskListId,
        [FromHeader(Name = "X-User-Id")] string? requesterUserId)
    {
        if (requesterUserId is null)
            return BadRequest("Missing X-User-Id header");

        var userIds = await sharingService.GetSharedUserIdsAsync(taskListId, requesterUserId);
        return Ok(userIds);
    }
}