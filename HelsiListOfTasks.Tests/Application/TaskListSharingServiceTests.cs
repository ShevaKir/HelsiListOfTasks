using HelsiListOfTasks.Application.Services;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Tests.Application;

[TestFixture]
public class TaskListSharingServiceTests
{
    private Mock<ITaskListRepository> _taskListRepositoryMock = null!;
    private Mock<ITaskListSharingRepository> _sharingRepositoryMock = null!;
    private TaskListSharingService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _taskListRepositoryMock = new Mock<ITaskListRepository>();
        _sharingRepositoryMock = new Mock<ITaskListSharingRepository>();
        _service = new TaskListSharingService(_taskListRepositoryMock.Object, _sharingRepositoryMock.Object);
    }

    [Test]
    public async Task AddShareAsync_ShouldReturnTrue_WhenOwnerMatches()
    {
        var taskList = new TaskList { Id = "1", OwnerId = "owner" };
        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.AddShareAsync("1", "owner", "user123");

        Assert.That(result, Is.True);
        _sharingRepositoryMock.Verify(x => x.AddShareAsync("1", "user123"), Times.Once);
    }

    [Test]
    public async Task AddShareAsync_ShouldReturnFalse_WhenOwnerDoesNotMatch()
    {
        var taskList = new TaskList { Id = "1", OwnerId = "someone_else" };
        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.AddShareAsync("1", "owner", "user123");

        Assert.That(result, Is.False);
        _sharingRepositoryMock.Verify(x => x.AddShareAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task RemoveShareAsync_ShouldReturnTrue_WhenRequesterIsOwner()
    {
        var taskList = new TaskList { Id = "1", OwnerId = "owner" };
        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.RemoveShareAsync("1", "owner", "target");

        Assert.That(result, Is.True);
        _sharingRepositoryMock.Verify(x => x.RemoveShareAsync("1", "target"), Times.Once);
    }

    [Test]
    public async Task RemoveShareAsync_ShouldReturnTrue_WhenRequesterHasAccess()
    {
        var taskList = new TaskList
        {
            Id = "1",
            OwnerId = "owner",
            SharedWithUserIds = ["user"]
        };

        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.RemoveShareAsync("1", "owner", "target");

        Assert.That(result, Is.True);
        _sharingRepositoryMock.Verify(x => x.RemoveShareAsync("1", "target"), Times.Once);
    }
    
    [Test]
    public async Task RemoveShareAsync_ShouldAllowUserToRemoveThemself_WhenShared()
    {
        var taskList = new TaskList
        {
            Id = "1",
            OwnerId = "owner",
            SharedWithUserIds = ["user123"]
        };

        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.RemoveShareAsync("1", "user123", "user123");

        Assert.That(result, Is.True);
        _sharingRepositoryMock.Verify(x => x.RemoveShareAsync("1", "user123"), Times.Once);
    }


    [Test]
    public async Task RemoveShareAsync_ShouldReturnFalse_WhenRequesterHasNoAccess()
    {
        var taskList = new TaskList { Id = "1", OwnerId = "owner", SharedWithUserIds = new List<string>() };
        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.RemoveShareAsync("1", "stranger", "target");

        Assert.That(result, Is.False);
        _sharingRepositoryMock.Verify(x => x.RemoveShareAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetSharedUserIdsAsync_ShouldReturnList_WhenRequesterIsOwner()
    {
        var taskList = new TaskList
        {
            Id = "1",
            OwnerId = "owner",
            SharedWithUserIds = new List<string> { "u1", "u2" }
        };

        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);
        _sharingRepositoryMock.Setup(x => x.GetSharedUserIdsAsync("1")).ReturnsAsync(taskList.SharedWithUserIds);

        var result = await _service.GetSharedUserIdsAsync("1", "owner");

        Assert.That(result, Is.EquivalentTo(new[] { "u1", "u2" }));
    }

    [Test]
    public async Task GetSharedUserIdsAsync_ShouldReturnEmpty_WhenAccessDenied()
    {
        var taskList = new TaskList
        {
            Id = "1",
            OwnerId = "owner",
            SharedWithUserIds = new List<string>()
        };

        _taskListRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.GetSharedUserIdsAsync("1", "stranger");

        Assert.That(result, Is.Empty);
        _sharingRepositoryMock.Verify(x => x.GetSharedUserIdsAsync(It.IsAny<string>()), Times.Never);
    }
}
