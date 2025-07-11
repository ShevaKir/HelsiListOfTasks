namespace HelsiListOfTasks.Domain.Models;

public class TaskList
{
    public int Id { get; init; }
    public string Title { get; set; }
    public int OwnerId { get; init; }
    public DateTime CreatedAt { get; set; }
    public List<int> SharedWithUserIds { get; set; }
}