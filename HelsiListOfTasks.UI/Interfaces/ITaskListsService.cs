using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.UI.Interfaces;

public interface ITaskListsService
{
    Task<List<TaskList>> GetTaskListsAsync(string userId);
}