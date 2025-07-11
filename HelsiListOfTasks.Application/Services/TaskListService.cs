using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Application.Services;

public class TaskListService(ITaskListRepository repository) : ITaskListService
{
    public async Task<TaskList> CreateAsync(TaskList taskList)
    {
        taskList.CreatedAt = DateTime.UtcNow;
        await repository.CreateAsync(taskList);
        return taskList;
    }

    public async Task<TaskList?> GetByIdAsync(int id, int userId)
    {
        var list = await repository.GetByIdAsync(id);
        return list.OwnerId != userId ? null : list;
    }

    public Task<List<TaskList>> GetForUserAsync(int userId)
    {
        return repository.GetByOwnerAsync(userId);
    }

    public async Task<bool> UpdateAsync(TaskList updatedList, int userId)
    {
        var existing = await repository.GetByIdAsync(updatedList.Id);
        if (existing.OwnerId != userId) return false;

        return await repository.UpdateAsync(updatedList);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing.OwnerId != userId) return false;

        return await repository.DeleteAsync(id);
    }
}