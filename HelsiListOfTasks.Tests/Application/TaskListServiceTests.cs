using HelsiListOfTasks.Application.Services;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Tests.Application;

public class TaskListServiceTests
{
    private Mock<ITaskListRepository> _repositoryMock = null!;
    private TaskListService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskListRepository>();
        _service = new TaskListService(_repositoryMock.Object);
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnList_WhenUserIsOwner()
    {
        var taskList = new TaskList { Id = "1", OwnerId = "123" };
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.GetByIdAsync("1", "123");

        Assert.That(result, Is.EqualTo(taskList));
    }


    [Test]
    public async Task GetByIdAsync_Should_ReturnNull_WhenUserIsNotOwner()
    {
        var taskList = new TaskList { Id = "1", OwnerId = "999" };
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(taskList);

        var result = await _service.GetByIdAsync("1", "123");

        Assert.IsNull(result);
    }

    [Test]
    public async Task GetByIdAsync_Should_ReturnNull_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("1"))!.ReturnsAsync((TaskList?)null);

        var result = await _service.GetByIdAsync("1", "123");

        Assert.IsNull(result);
    }

    [Test]
    public async Task GetForUserAsync_Should_ReturnUserLists()
    {
        var lists = new List<TaskList> { new() { Id = "1", OwnerId = "123" } };
        _repositoryMock.Setup(r => r.GetByOwnerAsync("123")).ReturnsAsync(lists);

        var result = await _service.GetForUserAsync("123");

        Assert.That(result, Is.EqualTo(lists));
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnFalse_WhenNotOwner()
    {
        var existing = new TaskList { Id = "1", OwnerId = "999" };
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(existing);

        var result = await _service.UpdateAsync(new TaskList { Id = "1" }, "123");

        Assert.IsFalse(result);
    }

    [Test]
    public async Task UpdateAsync_Should_ReturnFalse_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("1"))!.ReturnsAsync((TaskList?)null);

        var result = await _service.UpdateAsync(new TaskList { Id = "1" }, "123");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_Should_CallRepo_WhenOwner()
    {
        var updated = new TaskList { Id = "1", OwnerId = "123" };
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(updated);
        _repositoryMock.Setup(r => r.UpdateAsync(updated)).ReturnsAsync(true);

        var result = await _service.UpdateAsync(updated, "123");

        Assert.IsTrue(result);
        _repositoryMock.Verify(r => r.UpdateAsync(updated), Times.Once);
    }


    [Test]
    public async Task DeleteAsync_Should_ReturnFalse_WhenNotOwner()
    {
        var existing = new TaskList { Id = "1", OwnerId = "999" };
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(existing);

        var result = await _service.DeleteAsync("1", "123");

        Assert.IsFalse(result);
    }

    [Test]
    public async Task DeleteAsync_Should_ReturnFalse_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("1"))!.ReturnsAsync((TaskList?)null);

        var result = await _service.DeleteAsync("1", "123");

        Assert.IsFalse(result);
    }

    [Test]
    public async Task DeleteAsync_Should_CallRepo_WhenOwner()
    {
        var existing = new TaskList { Id = "1", OwnerId = "123" };
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.DeleteAsync("1")).ReturnsAsync(true);

        var result = await _service.DeleteAsync("1", "123");

        Assert.IsTrue(result);
        _repositoryMock.Verify(r => r.DeleteAsync("1"), Times.Once);
    }
}