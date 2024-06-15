using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config)
    {
        // Options pattern: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#the-options-pattern
        // Register it with DI via configure options: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#use-di-services-to-configure-options
        services.Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.SectionName));

        RegisterMongoDb(services);

        services.AddTransient<IPersonRepository, PersonRepository>();
        
        return services;
    }

    private static void RegisterMongoDb(IServiceCollection services)
    {

        // Per https://mongodb.github.io/mongo-csharp-driver/2.4/reference/driver/connecting/
        // It is recommended to store a MongoClient instance in a global place, either as a static variable
        // or in an IoC container with a singleton lifetime.
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            return new MongoClient(options.MongoDbConnectionString);
        });

        // The database is needed to create the collections
        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase(options.DatabaseName);
        });

        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

        // This doesn't appear to be necessary
        // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/serialization/guid-serialization/#guids
        //BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
        //BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
        //var setup = BsonSerializer.SerializerRegistry.GetSerializer<Guid>();

        RegisterMongoDbCollections(services);
        RegisterMongoDbClasses(services);
    }

    private static void RegisterMongoDbCollections(IServiceCollection services)
    {
        // Register the Person collection
        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var mongoDatabase = serviceProvider.GetRequiredService<IMongoDatabase>();
            return mongoDatabase.GetCollection<PersonModel>(options.CollectionName);
        });
    }
    private static void RegisterMongoDbClasses(IServiceCollection services)
    {
        // TODO:  Look into class registration
        // TODO:  How to setup indexes
        if (BsonClassMap.IsClassMapRegistered(typeof(PersonModel)) == false)
        {
            BsonClassMap.RegisterClassMap<PersonModel>(cm =>
            {
                cm.AutoMap();

                cm.MapMember(p => p.Id).SetIsRequired(true);
                cm.MapMember(p => p.CreatedAtUtc).SetIsRequired(true);

                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            });
        }
    }

   


}