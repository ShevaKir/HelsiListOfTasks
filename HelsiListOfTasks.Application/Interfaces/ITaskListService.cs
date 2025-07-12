using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Application.Interfaces;

public interface ITaskListService
{
    Task<TaskList> CreateAsync(TaskList taskList);
    Task<TaskList?> GetByIdAsync(string id, string userId);
    Task<List<TaskList>> GetForUserAsync(string userId);
    Task<bool> UpdateAsync(TaskList updatedList, string userId);
    Task<bool> DeleteAsync(string id, string userId);
}