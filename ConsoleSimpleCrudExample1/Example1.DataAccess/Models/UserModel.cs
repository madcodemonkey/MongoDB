using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Example1.DataAccess;
public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}