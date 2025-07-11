using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Domain.Repositories;

public interface ITaskListRepository
{
    Task<TaskList> GetByIdAsync(int id);
    Task<List<TaskList>> GetByOwnerAsync(int ownerId);
    Task CreateAsync(TaskList list);
    Task<bool> UpdateAsync(TaskList list);
    Task<bool> DeleteAsync(int id);
}