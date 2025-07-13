using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;
using MongoDB.Driver;

namespace HelsiListOfTasks.Infrastructure.Mongo;

public class MongoTaskListSharingRepository(MongoDbContext context) : ITaskListSharingRepository
{
    private readonly IMongoCollection<TaskList> _collection = context.TaskLists;

    public async Task AddShareAsync(string taskListId, string targetUserId)
    {
        var filter = Builders<TaskList>.Filter.Eq(x => x.Id, taskListId);
        var update = Builders<TaskList>.Update.AddToSet<string>(x => x.SharedWithUserIds, targetUserId);
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task RemoveShareAsync(string taskListId, string targetUserId)
    {
        var filter = Builders<TaskList>.Filter.Eq(x => x.Id, taskListId);
        var update = Builders<TaskList>.Update.Pull<string>(x => x.SharedWithUserIds, targetUserId);
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task<List<string>> GetSharedUserIdsAsync(string taskListId)
    {
        var taskList = await _collection
            .Find(x => x.Id == taskListId)
            .Project(x => x.SharedWithUserIds)
            .FirstOrDefaultAsync();

        return taskList ?? [];
    }
}