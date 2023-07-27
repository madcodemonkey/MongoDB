﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChangeMonitoring;

public class PersonModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; } = 18;

    public override string ToString()
    {
        return $"Id: {Id}  FirstName: {FirstName} LastName: {LastName}  Age: {Age}";
    }
}