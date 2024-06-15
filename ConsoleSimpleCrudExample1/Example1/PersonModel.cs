using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Example1;

public class PersonModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; } = 18;

    public override string ToString()
    {
        return $"Id: {Id}  FirstName: {FirstName} LastName: {LastName}  Age: {Age}";
    }
}