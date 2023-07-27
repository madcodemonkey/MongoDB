// Requires MongoDB.Driver NuGet package.

using Example1;
using Example1.DataAccess;
using MongoDB.Driver;

await SimplestThingEverAsync();

static async Task SimplestThingEverAsync()
{
    string connectionString = "mongodb://root:example@localhost:27017";
    string databaseName = "simple_db";
    string collectionName = "people";

    var client = new MongoClient(connectionString);
    var db = client.GetDatabase(databaseName);
    var collection = db.GetCollection<PersonModel>(collectionName);

    // Make sure that James Bond isn't already in the database.
    long numberOfHits = await collection.CountDocumentsAsync(w => w.FirstName == "James" && w.LastName == "Bond",
        new CountOptions { Limit = 1 });

    // Do we need to create the record?
    if (numberOfHits == 0)
    {
        var person = new PersonModel { FirstName = "James", LastName = "Bond" };

        await collection.InsertOneAsync(person);
    }

    // Get all the records in the collection and show them on the screen.
    var result = await collection.FindAsync(_ => true);

    foreach (var item in result.ToList())
    {
        Console.WriteLine(item.ToString());
    }
}

static async Task ChoreExampleAsync()
{
    ChoreDataAccess db = new ChoreDataAccess();
    await db.CreateUserAsync(new UserModel() { FirstName = "John", LastName = "Doe" });
    var users = await db.GetAllUsersAsync();
    
    var chore = new ChoreModel
    {
        AssignedTo = users.First(),
        ChoreText = "Mow the lawn",
        FrequencyInDays = 7,
    };
    await db.CreateChoreAsync(chore);
    
    var chores = await db.GetAllChoresAsync();
    var firstChore = chores.First();
    firstChore.LastCompleted = DateTime.UtcNow;
    await db.CompleteChoreAsync(firstChore);
}


