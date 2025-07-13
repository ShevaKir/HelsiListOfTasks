using HelsiListOfTasks.Domain.Models;

namespace HelsiListOfTasks.UI.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsersAsync();
}