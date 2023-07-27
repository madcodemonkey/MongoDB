// Requires MongoDB.Driver NuGet package.

using Example1;
using Example1.DataAccess;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

IConfiguration config = SetupConfiguration();

await SimplestThingEverAsync(config);
// await ChoreExampleAsync(config);

static async Task SimplestThingEverAsync(IConfiguration config)
{
    string connectionString = config["mongoDbConnectionString"];
    string databaseName = "simple_db";
    string collectionName = "people";

    var client = new MongoClient(connectionString);
    var db = client.GetDatabase(databaseName);
    var collection = db.GetCollection<PersonModel>(collectionName);

    var person = new PersonModel { FirstName = "Bob", LastName = "Hope" };

    // Make sure that person isn't already in the database.
    // Reference: https://stackoverflow.com/a/38211434/97803
    var findFilter = Builders<PersonModel>.Filter.And(
        Builders<PersonModel>.Filter.Eq(nameof(PersonModel.FirstName), person.FirstName),
        Builders<PersonModel>.Filter.Eq(nameof(PersonModel.LastName), person.LastName)
    );
    var foundPerson =  await (await collection.FindAsync<PersonModel>(findFilter)).FirstOrDefaultAsync();

    // Do we need to create the record?
    if (foundPerson == null)
    {
        await collection.InsertOneAsync(person);
    }
    else
    {
        foundPerson.Age += 1;
        var replaceFilter = Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), foundPerson.Id);
        await collection.ReplaceOneAsync(replaceFilter, foundPerson);
    }

    // Get all the records in the collection and show them on the screen.
    var result = await collection.FindAsync(_ => true);

    foreach (var item in result.ToList())
    {
        Console.WriteLine(item.ToString());
    }
}

static async Task ChoreExampleAsync(IConfiguration config)
{
    ChoreDataAccess db = new ChoreDataAccess(config);
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


static IConfiguration SetupConfiguration()
{
    // AddJsonFile requires:    Microsoft.Extensions.Configuration.Json NuGet package
    // AddUserSecrets requires: Microsoft.Extensions.Configuration.UserSecrets NuGet package
    // IConfiguration requires: Microsoft.Extensions.Configuration NuGet package (pulled in by previous NuGet)
    // https://stackoverflow.com/a/46437144/97803
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddUserSecrets<Program>();  // this is optional if you don't plan on using secrets

    IConfiguration config = builder.Build();

    return config;
}

