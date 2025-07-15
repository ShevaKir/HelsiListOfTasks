namespace HelsiListOfTasks.Application.Interfaces;

public interface ITaskListSharingService
{
    Task<bool> AddShareAsync(string taskListId, string ownerUserId, string targetUserId);
    Task<bool> RemoveShareAsync(string taskListId, string ownerId, string targetUserId);
    Task<List<string>> GetSharedUserIdsAsync(string taskListId, string ownerId);
}