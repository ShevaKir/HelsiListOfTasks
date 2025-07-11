using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;
using MongoDB.Driver;

namespace HelsiListOfTasks.Infrastructure.Mongo;

public class MongoUserRepository(MongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _collection = context.Users;

    public Task CreateAsync(User user)
    {
        return _collection.InsertOneAsync(user);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public Task<List<User>> GetAll()
    {
        return _collection.Find(FilterDefinition<User>.Empty).ToListAsync();
    }
}