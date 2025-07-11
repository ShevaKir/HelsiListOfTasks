using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Infrastructure.Mongo;
using MongoDB.Driver;

namespace HelsiListOfTasks.Tests;

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
        _taskLists =
        [
            new TaskList
            {
                Id = 1,
                Title = "Task 1",
                OwnerId = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                SharedWithUserIds = [200, 201]
            },

            new TaskList
            {
                Id = 2,
                Title = "Task 2",
                OwnerId = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                SharedWithUserIds = [202]
            },

            new TaskList
            {
                Id = 3,
                Title = "Task 3",
                OwnerId = 101,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                SharedWithUserIds = [200, 203]
            },

            new TaskList
            {
                Id = 4,
                Title = "Task 4",
                OwnerId = 102,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                SharedWithUserIds = []
            },

            new TaskList
            {
                Id = 5,
                Title = "Task 5",
                OwnerId = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                SharedWithUserIds = [201, 202, 203]
            }
        ];

        _client = new MongoClient("mongodb://localhost:27017");
        _client.DropDatabase(TestDatabaseName);
        _database = _client.GetDatabase(TestDatabaseName);

        var collection = _database.GetCollection<TaskList>("TaskLists");
        collection.InsertMany(_taskLists);

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
        var task = await _repository.GetByIdAsync(3);
        Assert.That(task, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(task.Id, Is.EqualTo(3));
            Assert.That(task.Title, Is.EqualTo("Task 3"));
        });
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_IfNotFound()
    {
        var task = await _repository.GetByIdAsync(999);
        Assert.That(task, Is.Null);
    }

    [Test]
    public async Task GetByOwnerAsync_ShouldReturnTasksSortedDescendingByCreatedAt()
    {
        int ownerId = 100;
        var tasks = await _repository.GetByOwnerAsync(ownerId);

        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks.Count, Is.EqualTo(3));

        for (int i = 0; i < tasks.Count - 1; i++)
        {
            Assert.That(tasks[i].CreatedAt, Is.GreaterThanOrEqualTo(tasks[i + 1].CreatedAt));
            Assert.That(tasks[i].OwnerId, Is.EqualTo(ownerId));
        }
    }

    [Test]
    public async Task CreateAsync_ShouldAddNewTask()
    {
        var newTask = new TaskList { Id = 10, Title = "New Task", OwnerId = 103, CreatedAt = DateTime.UtcNow };
        await _repository.CreateAsync(newTask);

        var fetched = await _repository.GetByIdAsync(newTask.Id);
        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched.Title, Is.EqualTo("New Task"));
        Assert.That(fetched.OwnerId, Is.EqualTo(103));
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyExistingTask()
    {
        var taskToUpdate = await _repository.GetByIdAsync(1);
        Assert.That(taskToUpdate, Is.Not.Null);

        taskToUpdate.Title = "Updated Title";
        await _repository.UpdateAsync(taskToUpdate);

        var updatedTask = await _repository.GetByIdAsync(1);
        Assert.That(updatedTask, Is.Not.Null);
        Assert.That(updatedTask.Title, Is.EqualTo("Updated Title"));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        var existingTask = await _repository.GetByIdAsync(2);
        Assert.That(existingTask, Is.Not.Null);

        await _repository.DeleteAsync(existingTask.Id);

        var deletedTask = await _repository.GetByIdAsync(existingTask.Id);
        Assert.That(deletedTask, Is.Null);
    }
}