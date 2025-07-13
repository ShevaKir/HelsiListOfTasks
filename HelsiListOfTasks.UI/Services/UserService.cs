using System.Text.Json;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.UI.Interfaces;

namespace HelsiListOfTasks.UI.Services;

public class UserService(HttpClient httpClient) : IUserService
{
    public async Task<List<User>> GetUsersAsync()
    {
        var response = await httpClient.GetAsync("users");
        if (!response.IsSuccessStatusCode)
            return [];

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }
}