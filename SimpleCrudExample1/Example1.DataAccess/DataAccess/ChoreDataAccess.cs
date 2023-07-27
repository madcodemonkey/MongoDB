using MongoDB.Driver;

namespace Example1.DataAccess;

public class ChoreDataAccess
{
    private const string ConnectionString = "mongodb://root:example@localhost:27017";
    private const string DatabaseName = "choredb";
    private const string ChoreCollection = "chore_chart";
    private const string UserCollection = "users";
    private const string ChoreHistoryCollection = "chore_history";


    private IMongoCollection<T> ConnectionToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        var usersCollection = ConnectionToMongo<UserModel>(UserCollection);
        var results = await usersCollection.FindAsync(_ => true);
        return await results.ToListAsync();
    }

    public async Task<List<ChoreModel>> GetAllChoresAsync()
    {
        var choresCollection = ConnectionToMongo<ChoreModel>(ChoreCollection);
        var results = await choresCollection.FindAsync(_ => true);
        return await results.ToListAsync();
    }

    public async Task<List<ChoreModel>> GetAllChoresForUserAsync(UserModel user)
    {
        var choresCollection = ConnectionToMongo<ChoreModel>(ChoreCollection);
        var results = await choresCollection.FindAsync(c => c.AssignedTo.Id == user.Id);
        return await results.ToListAsync();
    }

    public async Task CreateUserAsync(UserModel user)
    {
        var usersCollection = ConnectionToMongo<UserModel>(UserCollection);
        await usersCollection.InsertOneAsync(user);
    }

    public async Task CreateChoreAsync(ChoreModel chore)
    {
        var choresCollection = ConnectionToMongo<ChoreModel>(ChoreCollection);
        await choresCollection.InsertOneAsync(chore);
    }

    public async Task UpdateChoreAsync(ChoreModel chore)
    {
        var choresCollection = ConnectionToMongo<ChoreModel>(ChoreCollection);
        var filter = Builders<ChoreModel>.Filter.Eq(nameof(ChoreModel.Id), chore.Id);
        await choresCollection.ReplaceOneAsync(filter, chore, new ReplaceOptions() {IsUpsert = true});
    }

    public async Task CompleteChoreAsync(ChoreModel chore)
    {
        var choresCollection = ConnectionToMongo<ChoreModel>(ChoreCollection);
        var filter = Builders<ChoreModel>.Filter.Eq(nameof(ChoreModel.Id), chore.Id);
        await choresCollection.ReplaceOneAsync(filter, chore);

        var choreHistoryCollection = ConnectionToMongo<ChoreHistoryModel>(ChoreHistoryCollection);
        await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(chore));


        // Requires a cloud server to work with transactions!!
        // Requires a cloud server to work with transactions!!
        // Requires a cloud server to work with transactions!!
        // Requires a cloud server to work with transactions!!

        //var client = new MongoClient(ConnectionString);
        //using var session = await client.StartSessionAsync();
        //session.StartTransaction();

        //try
        //{
        //    var db = client.GetDatabase(DatabaseName);
        //    var choresCollection = db.GetCollection<ChoreModel>(ChoreCollection);
        //    var filter = Builders<ChoreModel>.Filter.Eq(nameof(ChoreModel.Id), chore.Id);
        //    await choresCollection.ReplaceOneAsync(filter, chore);

        //    var choreHistoryCollection = db.GetCollection<ChoreHistoryModel>(ChoreHistoryCollection);
        //    await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(chore));

        //    await session.CommitTransactionAsync();
        //}
        //catch (Exception ex)
        //{
        //    await session.AbortTransactionAsync();
        //    Console.WriteLine(ex.Message);
        //}
    }


    public async Task DeleteChoreAsync(ChoreModel chore)
    {
        var choresCollection = ConnectionToMongo<ChoreModel>(ChoreCollection);
        await choresCollection.DeleteOneAsync(c => c.Id == chore.Id);
    }

}
