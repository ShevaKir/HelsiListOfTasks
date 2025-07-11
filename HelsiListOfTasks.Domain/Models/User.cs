using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HelsiListOfTasks.Domain.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }

    public string Name { get; init; }
}