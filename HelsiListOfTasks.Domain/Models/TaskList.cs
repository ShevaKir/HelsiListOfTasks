using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HelsiListOfTasks.Domain.Models;

public class TaskList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    public string Title { get; set; }
    public string OwnerId { get; init; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<string> SharedWithUserIds { get; set; } = [];
}