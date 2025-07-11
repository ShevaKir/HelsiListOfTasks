using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Application.Interfaces;

public interface ITaskListService
{
    Task<TaskList> CreateAsync(TaskList taskList);
    Task<TaskList?> GetByIdAsync(string id, int userId);
    Task<List<TaskList>> GetForUserAsync(int userId);
    Task<bool> UpdateAsync(TaskList updatedList, int userId);
    Task<bool> DeleteAsync(string id, int userId);
}