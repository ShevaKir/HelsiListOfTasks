using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Infrastructure.Mongo;
using MongoDB.Driver;

namespace HelsiListOfTasks.Tests.Infrastructure;

public class MongoTaskListSharingRepositoryIntegrationTests
{
    private MongoClient _client;
    private IMongoDatabase _database;
    private MongoTaskListSharingRepository _repository;
    private IMongoCollection<TaskList> _collection;
    private const string TestDatabaseName = "Test_SharingDb";
    private TaskList _taskList;

    [SetUp]
    public void Setup()
    {
        _client = new MongoClient("mongodb://localhost:27017");
        _client.DropDatabase(TestDatabaseName);
        _database = _client.GetDatabase(TestDatabaseName);

        var context = new MongoDbContext(_database);
        _repository = new MongoTaskListSharingRepository(context);
        _collection = _database.GetCollection<TaskList>("TaskLists");

        _taskList = new TaskList
        {
            Title = "Shared Task",
            OwnerId = "100",
            CreatedAt = DateTime.UtcNow,
            SharedWithUserIds = new List<string> { "200" }
        };

        _collection.InsertOne(_taskList);
    }

    [TearDown]
    public void TearDown()
    {
        _client.DropDatabase(TestDatabaseName);
        _client.Dispose();
    }

    [Test]
    public async Task AddShareAsync_ShouldAddUserToSharedWithUserIds()
    {
        await _repository.AddShareAsync(_taskList.Id, "201");

        var updated = await _collection.Find(x => x.Id == _taskList.Id).FirstOrDefaultAsync();

        Assert.That(updated.SharedWithUserIds, Does.Contain("201"));
    }

    [Test]
    public async Task RemoveShareAsync_ShouldRemoveUserFromSharedWithUserIds()
    {
        await _repository.RemoveShareAsync(_taskList.Id, "200");

        var updated = await _collection.Find(x => x.Id == _taskList.Id).FirstOrDefaultAsync();

        Assert.That(updated.SharedWithUserIds, Does.Not.Contain("200"));
    }

    [Test]
    public async Task GetSharedUserIdsAsync_ShouldReturnCorrectUsers()
    {
        var result = await _repository.GetSharedUserIdsAsync(_taskList.Id);

        Assert.That(result, Is.EquivalentTo(new List<string> { "200" }));
    }
}
