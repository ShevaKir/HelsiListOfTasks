namespace HelsiListOfTasks.Domain.Models;

public class TaskItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}