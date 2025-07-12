using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HelsiListOfTasks.Tests.Infrastructure;

public class MongoUserRepositoryIntegrationTests
{
    private MongoClient _client;
    private IMongoDatabase _database;
    private MongoUserRepository _repository;
    private List<User> _users;
    private const string TestDatabaseName = "Test_TaskListDb";

    [SetUp]
    public void Setup()
    {
        _client = new MongoClient("mongodb://localhost:27017");
        _client.DropDatabase(TestDatabaseName);
        _database = _client.GetDatabase(TestDatabaseName);

        var collection = _database.GetCollection<User>("Users");

        var seedData = new List<User>
        {
            new User()
            {
                Name = "User 1"
            },
            new User()
            {
                Name = "User 2"
            },
            new User()
            {
                Name = "User 3"
            },
        };

        collection.InsertMany(seedData);
        _users = collection.Find(FilterDefinition<User>.Empty).ToList();
        var context = new MongoDbContext(_database);

        _repository = new MongoUserRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        _client.DropDatabase(TestDatabaseName);
        _client.Dispose();
    }

    [Test]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        var result = await _repository.GetAll();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(u => u.Name), Is.EquivalentTo(_users.Select(u => u.Name)));
    }
    
    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectTask()
    {
        var task1 = _users.First(x => x.Name == "User 1");
        var task = await _repository.GetByIdAsync(task1.Id);

        Assert.That(task, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(task.Id, Is.EqualTo(task1.Id));
            Assert.That(task1.Name, Is.EqualTo("User 1"));
        });
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_IfNotFound()
    {
        var fakeId = ObjectId.GenerateNewId().ToString();

        var result = await _repository.GetByIdAsync(fakeId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_ShouldAddNewUser()
    {
        var newUser = new User { Name = "New User" };

        await _repository.CreateAsync(newUser);

        var users = await _repository.GetAll();
        Assert.That(users.Any(u => u.Name == "New User"), Is.True);
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        var userToDelete = _users.First();

        var success = await _repository.DeleteAsync(userToDelete.Id);

        Assert.That(success, Is.True);

        var users = await _repository.GetAll();
        Assert.That(users.Any(u => u.Id == userToDelete.Id), Is.False);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnFalse_IfUserDoesNotExist()
    {
        var nonExistentId = "000000000000000000000000"; // valid format, but not real

        var result = await _repository.DeleteAsync(nonExistentId);

        Assert.That(result, Is.False);
    }
}