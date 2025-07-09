namespace HelsiListOfTasks.Domain.Models;

public class TaskList
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<int> SharedWithUserIds { get; set; }
}