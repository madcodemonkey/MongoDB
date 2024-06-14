using MongoDB.Driver;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Data;

public class PersonRepository : IPersonRepository
{
    private readonly IMongoCollection<PersonModel> _mongoCollection;

    /// <summary>Constructor</summary>
    public PersonRepository(IMongoCollection<PersonModel> mongoCollection)
    {
        _mongoCollection = mongoCollection;
    }

    /// <inheritdoc/>
    public async Task<PersonModel> AddAsync(PersonModel person, CancellationToken cancellationToken = default)
    {
        if (person.Id == Guid.Empty)
        {
            person.Id = Guid.NewGuid();
        }
        else if (await ExistsAsync(person.Id, cancellationToken))
        {
            throw new Exception("Person already exists");
        }

        person.CreatedAtUtc = DateTime.UtcNow;
        person.LastModifiedAtUtc = DateTime.UtcNow;
        
        await _mongoCollection.InsertOneAsync(person, options: null, cancellationToken);
        return person;
    }

    /// <inheritdoc/>
    public async Task<PersonModel?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/read-operations/retrieve/

        // If there are lots of items, see this example of walking through in batches:
        // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/read-operations/retrieve/#example
        var filter = Builders<PersonModel>.Filter
            .And(Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), id));

        var person = await _mongoCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return person;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/read-operations/count/#accurate-count
        var filter = Builders<PersonModel>.Filter
            .And(Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), id));
        
        var numberOfDocs = await _mongoCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return numberOfDocs > 0;
    }

    /// <inheritdoc/>
    public async Task<ICollection<PersonModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<PersonModel>.Filter.Empty;

        var personList = await _mongoCollection.Find(filter).ToListAsync(cancellationToken);

        return personList ?? new List<PersonModel>(0);
    }

    /// <inheritdoc/>
    public async Task<PersonModel> UpdateAsync(Guid id, PersonModel person, CancellationToken cancellationToken = default)
    {
        person.Id = id;
        person.LastModifiedAtUtc = DateTime.UtcNow;

        var filter = Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), id);

        await _mongoCollection.ReplaceOneAsync(filter, person, cancellationToken: cancellationToken);
        return person;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), id);

        await _mongoCollection.DeleteOneAsync(filter, cancellationToken);
    }
}