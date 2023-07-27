// Requires MongoDB.Driver NuGet package.
using ChangeMonitoring;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Monitor has started....");

IConfiguration config = SetupConfiguration();
string connectionString = config["mongoDbAtlasConnectionString"];

if (connectionString.StartsWith("--"))
    throw new ArgumentException("Please update the MongoDB atlas connection string!");

string databaseName = "simple_db";
string collectionName = "people";

var client = new MongoClient(connectionString);

var tokenSource = new CancellationTokenSource();
string? resumeToken = null; // "8264C2CB7D000000032B022C0100296E5A100429E95A8BDF404BA9A25A43FB547459D448971F6964006464C2CA4B583096DCF865FB780004";
await MonitorOneDatabasesAsync(client, databaseName, resumeToken, tokenSource.Token);

//await ShowPeopleAsync(client, databaseName, collectionName);

Console.WriteLine("Monitor has exited!!");

static async Task MonitorOneDatabasesAsync(IMongoClient client, string monitorDatabaseName, string? resumeToken,
    CancellationToken cancellationToken = default)
{
    var options = new ChangeStreamOptions
    {
        FullDocument = ChangeStreamFullDocumentOption.Default, 
        ResumeAfter = string.IsNullOrWhiteSpace(resumeToken) ? null : new BsonDocument().Add("_data", resumeToken)
    };
   

    IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> streamCursor = await client
        .GetDatabase(monitorDatabaseName)
        .WatchAsync(options, cancellationToken);
    
    foreach (ChangeStreamDocument<BsonDocument> changeItem in streamCursor.ToEnumerable())
    {
        Console.WriteLine($"Key that changed: {changeItem.DocumentKey}  Operation Type: {changeItem.OperationType}");
        Console.WriteLine($"Resume Token: {changeItem.ResumeToken["_data"]}");

        // Delete doesn't send the full document!
        if (changeItem.FullDocument != null)
        {
            // Show all the fields on the document.
            foreach (string name in changeItem.FullDocument.Names)
            {
                Console.WriteLine($"  {name}: {changeItem.FullDocument[name]}");
            }
        }

        if (cancellationToken.IsCancellationRequested)
            break;
    }
}

static async Task ShowPeopleAsync(IMongoClient client, string databaseName, string collectionName)
{
    var db = client.GetDatabase(databaseName);

    var collection = db.GetCollection<PersonModel>(collectionName);

    // Get all the records in the collection and show them on the screen.
    var result = await collection.FindAsync(_ => true);

    foreach (var item in result.ToList())
    {
        Console.WriteLine(item.ToString());
    }
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

