using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.Domain.Repositories;

namespace HelsiListOfTasks.Application.Services;

public class UserService(IUserRepository userRepository, ITaskListRepository taskListRepository) : IUserService
{
    public Task<List<User>> GetAllAsync()
    {
        return userRepository.GetAll();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user;  
    }

    public async Task<User> CreateAsync(User user)
    {
        await userRepository.CreateAsync(user);
        return user;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var taskLists = await taskListRepository.GetByOwnerAsync(id);

        foreach (var taskList in taskLists)
        {
            await taskListRepository.DeleteAsync(taskList.Id);
        }

        // TODO: Clean up SharedWithUserIds when feature is implemented

        return await userRepository.DeleteAsync(id);
    }
}