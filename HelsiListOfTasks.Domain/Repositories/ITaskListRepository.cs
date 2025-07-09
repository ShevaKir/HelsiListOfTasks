using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Domain.Repositories;

public interface ITaskListRepository
{
    Task<List<TaskList>> GetAccessibleListsAsync(int userId);
    Task<TaskList?> GetByIdAsync(int id);
    Task CreateAsync(TaskList list);
    Task UpdateAsync(TaskList list);
    Task DeleteAsync(int id);
}