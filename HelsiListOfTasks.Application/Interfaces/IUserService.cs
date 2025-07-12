using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.Application.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User> CreateAsync(User user);
    Task<bool> DeleteAsync(string id);
}