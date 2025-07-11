using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Application.Services;

public class TaskListService(ITaskListRepository repository) : ITaskListService
{
    public async Task<TaskList> CreateAsync(TaskList taskList)
    {
        await repository.CreateAsync(taskList);
        return taskList;
    }

    public async Task<TaskList?> GetByIdAsync(string id, string userId)
    {
        var list = await repository.GetByIdAsync(id);
        if (list is null || list.OwnerId != userId)
            return null;

        return list;
    }

    public Task<List<TaskList>> GetForUserAsync(string userId)
    {
        return repository.GetByOwnerAsync(userId);
    }

    public async Task<bool> UpdateAsync(TaskList updatedList, string userId)
    {
        var existing = await repository.GetByIdAsync(updatedList.Id);
        if (existing is null || existing.OwnerId != userId)
            return false;

        return await repository.UpdateAsync(updatedList);
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null || existing.OwnerId != userId)
            return false;

        return await repository.DeleteAsync(id);
    }
}