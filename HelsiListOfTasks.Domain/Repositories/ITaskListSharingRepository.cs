namespace HelsiListOfTasks.Domain.Repositories;

public interface ITaskListSharingRepository
{
    Task AddShareAsync(string taskListId, string targetUserId);
    Task RemoveShareAsync(string taskListId, string targetUserId);
    Task<List<string>> GetSharedUserIdsAsync(string taskListId);
}