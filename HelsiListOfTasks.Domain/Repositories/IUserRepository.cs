using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Domain.Repositories;

public interface IUserRepository
{
    Task CreateAsync(User list);
    Task<bool> DeleteAsync(string id);
    Task<List<User>> GetAll();
}