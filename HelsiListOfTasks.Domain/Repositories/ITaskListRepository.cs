using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Domain.Repositories;

public interface ITaskListRepository
{
    Task<TaskList?> GetByIdAsync(string id);
    Task<List<TaskList>> GetByOwnerAsync(string ownerId);
    Task<List<TaskList>> GetAccessibleListsAsync(string userId);
    Task<List<TaskList>> GetAllWithSharedUserAsync(string userId);
    Task CreateAsync(TaskList list);
    Task<bool> UpdateAsync(TaskList list);
    Task<bool> DeleteAsync(string id);
}