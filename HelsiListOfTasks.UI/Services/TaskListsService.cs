using System.Text.Json;
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.UI.Interfaces;

namespace HelsiListOfTasks.UI.Services;

public class TaskListsService(HttpClient httpClient) : ITaskListsService
{
    public async Task<List<TaskList>> GetTaskListsAsync(string userId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "task-lists");
        request.Headers.Add("X-User-Id", userId);

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return [];

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TaskList>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }
}