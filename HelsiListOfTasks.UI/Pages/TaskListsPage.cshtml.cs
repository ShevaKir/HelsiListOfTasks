using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.UI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelsiListOfTasks.UI.Pages;

public class TaskListsPageModel(ITaskListsService taskListsService) : PageModel
{
    public List<TaskList> TaskLists { get; private set; } = [];
    [BindProperty(SupportsGet = true)] public string? UserId { get; set; }

    public async Task OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(UserId))
        {
            TaskLists = [];
            return;
        }

        TaskLists = await taskListsService.GetTaskListsAsync(UserId);
    }
}