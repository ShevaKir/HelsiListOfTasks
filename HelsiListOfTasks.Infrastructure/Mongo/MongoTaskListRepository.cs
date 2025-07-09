using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Infrastructure.Mongo;

public class MongoTaskListRepository : ITaskListRepository
{
    public Task<List<TaskList>> GetAccessibleListsAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<TaskList?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(TaskList list)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TaskList list)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}