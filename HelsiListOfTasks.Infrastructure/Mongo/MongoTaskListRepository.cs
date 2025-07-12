using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;
using MongoDB.Driver;

namespace HelsiListOfTasks.Infrastructure.Mongo;

public class MongoTaskListRepository(MongoDbContext context) : ITaskListRepository
{
    private readonly IMongoCollection<TaskList> _collection = context.TaskLists;

    public Task<TaskList?> GetByIdAsync(string id)
    {
        return _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public Task<List<TaskList>> GetByOwnerAsync(string ownerId)
    {
        return _collection
            .Find(x => x.OwnerId == ownerId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task CreateAsync(TaskList list)
    {
        //TODO: Check if there is a user who wants to create a task 
        return _collection.InsertOneAsync(list);
    }

    public async Task<bool> UpdateAsync(TaskList list)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == list.Id, list);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}