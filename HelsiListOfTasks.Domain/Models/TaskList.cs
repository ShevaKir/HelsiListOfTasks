namespace HelsiListOfTasks.Domain.Models;

public class TaskList
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<int> SharedWithUserIds { get; set; }
}