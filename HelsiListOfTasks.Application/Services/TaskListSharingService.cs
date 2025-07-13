using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Application.Services;

public class TaskListSharingService(
    ITaskListRepository taskListRepository,
    ITaskListSharingRepository sharingRepository) : ITaskListSharingService
{
    public async Task<bool> AddShareAsync(string taskListId, string ownerUserId, string targetUserId)
    {
        var taskList = await taskListRepository.GetByIdAsync(taskListId);
        if (taskList is null || taskList.OwnerId != ownerUserId)
            return false;

        await sharingRepository.AddShareAsync(taskListId, targetUserId);
        return true;
    }

    public async Task<bool> RemoveShareAsync(string taskListId, string requesterUserId, string targetUserId)
    {
        var taskList = await taskListRepository.GetByIdAsync(taskListId);
        if (taskList is null ||
            (taskList.OwnerId != requesterUserId && !taskList.SharedWithUserIds.Contains(requesterUserId)))
            return false;

        await sharingRepository.RemoveShareAsync(taskListId, targetUserId);
        return true;
    }

    public async Task<List<string>> GetSharedUserIdsAsync(string taskListId, string requesterUserId)
    {
        var taskList = await taskListRepository.GetByIdAsync(taskListId);
        if (taskList is null ||
            (taskList.OwnerId != requesterUserId && !taskList.SharedWithUserIds.Contains(requesterUserId)))
            return [];

        return await sharingRepository.GetSharedUserIdsAsync(taskListId);
    }
}