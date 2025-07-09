namespace HelsiListOfTasks.Domain.Repositories;

public interface ITaskListSharingRepository
{
    Task AddShareAsync(int listId, int targetUserId);
    Task RemoveShareAsync(int listId, int targetUserId);
    Task<List<int>> GetSharedUserIdsAsync(int listId);
}