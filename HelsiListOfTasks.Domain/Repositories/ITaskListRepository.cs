using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Domain.Repositories;

public interface ITaskListRepository
{
    Task<TaskList> GetByIdAsync(int id);
    Task<List<TaskList>> GetByOwnerAsync(int ownerId);
    Task CreateAsync(TaskList list);
    Task UpdateAsync(TaskList list);
    Task DeleteAsync(int id);
}