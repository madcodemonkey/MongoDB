using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Data.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task ApplyMongoDbChangesAsync(this IServiceProvider serviceProvider)
    {
        var collection = serviceProvider.GetRequiredService<IMongoCollection<PersonModel>>();
        var indexKeysDefinition = Builders<PersonModel>.IndexKeys.Ascending(x => x.FirstName).Ascending(x => x.LastName);
        var indexOptions = new CreateIndexOptions { Unique = true, Name = "FirstAndLastName"};
        var indexModel = new CreateIndexModel<PersonModel>(indexKeysDefinition, indexOptions);
        await collection.Indexes.CreateOneAsync(indexModel);
    }
}