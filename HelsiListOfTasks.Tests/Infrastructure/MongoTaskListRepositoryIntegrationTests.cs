using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HelsiListOfTasks.Tests.Infrastructure;

public class MongoTaskListRepositoryIntegrationTests
{
    private MongoClient _client;
    private IMongoDatabase _database;
    private MongoTaskListRepository _repository;
    private List<TaskList> _taskLists;
    private const string TestDatabaseName = "Test_TaskListDb";

    [SetUp]
    public void Setup()
    {
        _client = new MongoClient("mongodb://localhost:27017");
        _client.DropDatabase(TestDatabaseName);
        _database = _client.GetDatabase(TestDatabaseName);

        var collection = _database.GetCollection<TaskList>("TaskLists");

        var seedData = new List<TaskList>
        {
            new TaskList
            {
                Title = "Task 1",
                OwnerId = "100",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                SharedWithUserIds = ["200", "201"]
            },
            new TaskList
            {
                Title = "Task 2",
                OwnerId = "100",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                SharedWithUserIds = ["202"]
            },
            new TaskList
            {
                Title = "Task 3",
                OwnerId = "101",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                SharedWithUserIds = ["200", "203"]
            },
            new TaskList
            {
                Title = "Task 4",
                OwnerId = "102",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                SharedWithUserIds = []
            },
            new TaskList
            {
                Title = "Task 5",
                OwnerId = "100",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                SharedWithUserIds = ["201", "202", "203"]
            }
        };

        collection.InsertMany(seedData);

        _taskLists = collection.Find(FilterDefinition<TaskList>.Empty).ToList();

        var context = new MongoDbContext(_database);
        _repository = new MongoTaskListRepository(context);
    }


    [TearDown]
    public void TearDown()
    {
        _client.DropDatabase(TestDatabaseName);
        _client.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectTask()
    {
        var task3 = _taskLists.First(x => x.Title == "Task 3");
        var task = await _repository.GetByIdAsync(task3.Id);

        Assert.That(task, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(task.Id, Is.EqualTo(task3.Id));
            Assert.That(task.Title, Is.EqualTo("Task 3"));
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
    public async Task GetByOwnerAsync_ShouldReturnTasksSortedDescendingByCreatedAt()
    {
        const string ownerId = "100";
        var expected = _taskLists
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        var actual = await _repository.GetByOwnerAsync(ownerId);

        Assert.That(actual, Has.Count.EqualTo(expected.Count));

        for (var i = 0; i < expected.Count; i++)
        {
            Assert.That(actual[i].Id, Is.EqualTo(expected[i].Id));
        }
    }

    [Test]
    public async Task GetAccessibleListsAsync_ShouldReturnOwnedAndSharedLists_SortedByCreatedAt()
    {
        var userId = "200";

        var expected = _taskLists
            .Where(x => x.OwnerId == userId || x.SharedWithUserIds.Contains(userId))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .ToList();

        var result = await _repository.GetAccessibleListsAsync(userId);

        var actual = result.Select(x => x.Id).ToList();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetPagedForUserAsync_ShouldReturnCorrectPage()
    {
        var userId = "200";
        var all = await _repository.GetAccessibleListsAsync(userId);

        var expectedPage = all.Skip(0).Take(2).Select(x => x.Id).ToList();

        var page = await _repository.GetPagedForUserAsync(userId, 0, 2);
        var actual = page.Select(x => x.Id).ToList();

        Assert.That(actual, Is.EqualTo(expectedPage));
    }
    
    [Test]
    public async Task GetAllWithSharedUserAsync_ShouldReturnOnlySharedLists()
    {
        var userId = "203";

        var expected = _taskLists
            .Where(x => x.SharedWithUserIds.Contains(userId))
            .Select(x => x.Id)
            .ToList();

        var result = await _repository.GetAllWithSharedUserAsync(userId);
        var actual = result.Select(x => x.Id).ToList();

        Assert.That(actual, Is.EqualTo(expected));
    }



    [Test]
    public async Task CreateAsync_ShouldAddNewTask()
    {
        var newTask = new TaskList
        {
            Title = "New Task",
            OwnerId = "103",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(newTask);

        var fromDb = await _repository.GetByIdAsync(newTask.Id);

        Assert.That(fromDb, Is.Not.Null);
        Assert.Multiple(() => { Assert.That(fromDb.Title, Is.EqualTo("New Task")); });

        Assert.That(fromDb.OwnerId, Is.EqualTo("103"));
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyExistingTask()
    {
        var original = _taskLists.First(x => x.Title == "Task 1");
        original.Title = "Updated Task 1";

        var result = await _repository.UpdateAsync(original);
        Assert.That(result, Is.True);

        var updated = await _repository.GetByIdAsync(original.Id);
        Assert.That(updated?.Title, Is.EqualTo("Updated Task 1"));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        var toDelete = _taskLists.First(x => x.Title == "Task 2");

        var result = await _repository.DeleteAsync(toDelete.Id);
        Assert.That(result, Is.True);

        var deleted = await _repository.GetByIdAsync(toDelete.Id);
        Assert.That(deleted, Is.Null);
    }
}