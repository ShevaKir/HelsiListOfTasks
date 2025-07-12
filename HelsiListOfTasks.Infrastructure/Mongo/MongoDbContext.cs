using HelsiListOfTasks.Domain.Models;
using MongoDB.Driver;

namespace HelsiListOfTasks.Infrastructure.Mongo;

public class MongoDbContext(IMongoDatabase database)
{
    private readonly IMongoDatabase _database = database ?? throw new ArgumentNullException(nameof(database));

    public IMongoCollection<TaskList> TaskLists => _database.GetCollection<TaskList>("TaskLists");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
}