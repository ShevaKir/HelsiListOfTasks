using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;
using MongoDB.Driver;

namespace HelsiListOfTasks.Infrastructure.Mongo;

public class MongoTaskListRepository(MongoDbContext context) : ITaskListRepository
{
    private readonly IMongoCollection<TaskList> _collection = context.TaskLists;

    public Task<TaskList> GetByIdAsync(int id)
    {
        return _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<List<TaskList>> GetByOwnerAsync(int ownerId)
    {
        return _collection
            .Find(x => x.OwnerId == ownerId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task CreateAsync(TaskList list)
    {
        return _collection.InsertOneAsync(list);
    }

    public Task UpdateAsync(TaskList list)
    {
        return _collection.ReplaceOneAsync(x => x.Id == list.Id, list);
    }

    public Task DeleteAsync(int id)
    {
        return _collection.DeleteOneAsync(x => x.Id == id);
    }
}