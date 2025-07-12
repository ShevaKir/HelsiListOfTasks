using HelsiListOfTasks.Application.Services;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Tests.Application;

public class UserServiceTests
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<ITaskListRepository> _taskListRepoMock;
    private UserService _service;

    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _taskListRepoMock = new Mock<ITaskListRepository>();
        _service = new UserService(_userRepoMock.Object, _taskListRepoMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        var users = new List<User> { new() { Name = "Test" } };
        _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.That(result, Is.EqualTo(users));
        _userRepoMock.Verify(r => r.GetAll(), Times.Once);
    }
    
    [Test]
    public async Task GetByIdAsync_Should_ReturnUser()
    {
        var user = new User() { Id = "1", Name = "User 1" };
        _userRepoMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var result = await _service.GetByIdAsync("1");

        Assert.That(result, Is.EqualTo(user));
    }


    [Test]
    public async Task GetByIdAsync_Should_ReturnNull()
    {
        var user = new User() { Id = "1", Name = "User"};
        _userRepoMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var result = await _service.GetByIdAsync("2");

        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task CreateAsync_ShouldCallRepository()
    {
        var user = new User { Name = "New User" };

        await _service.CreateAsync(user);

        _userRepoMock.Verify(r => r.CreateAsync(user), Times.Once);
    }
    
    [Test]
    public async Task DeleteAsync_ShouldDeleteUserAndTheirTaskLists()
    {
        const string userId = "123";
        var taskLists = new List<TaskList>
        {
            new TaskList { Id = "1", OwnerId = "123" },
            new TaskList { Id = "2", OwnerId = "123" }
        };

        _taskListRepoMock.Setup(r => r.GetByOwnerAsync(It.IsAny<string>()))
            .ReturnsAsync(taskLists);
        _taskListRepoMock.Setup(r => r.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _userRepoMock.Setup(r => r.DeleteAsync(userId))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(userId);

        Assert.That(result, Is.True);
        _taskListRepoMock.Verify(r => r.GetByOwnerAsync(userId), Times.Once);
        _taskListRepoMock.Verify(r => r.DeleteAsync("1"), Times.Once);
        _taskListRepoMock.Verify(r => r.DeleteAsync("2"), Times.Once);
        _userRepoMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }
}