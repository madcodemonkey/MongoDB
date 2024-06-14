using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApiCrudExample.Model;

public class PersonModel
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; } = 18;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAtUtc { get; set; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"Id: {Id}  FirstName: {FirstName} LastName: {LastName}  Age: {Age}";
    }
}