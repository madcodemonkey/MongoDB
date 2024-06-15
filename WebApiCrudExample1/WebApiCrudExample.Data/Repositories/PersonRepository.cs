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
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), id);

        await _mongoCollection.DeleteOneAsync(filter, cancellationToken);
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
    public async Task<bool> ExistsAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PersonModel>.Filter.Eq(nameof(PersonModel.FirstName), firstName) &
                     Builders<PersonModel>.Filter.Eq(nameof(PersonModel.LastName), lastName);

        var findOptions = CreateCaseInsensitiveFindOptions();

        var count = await _mongoCollection
            .Find(filter, findOptions)
            .CountDocumentsAsync(cancellationToken);

        return count > 0;
    }

    /// <inheritdoc/>
    public async Task<List<PersonModel>> FindByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PersonModel>.Filter.Eq(nameof(PersonModel.FirstName), firstName) &
                     Builders<PersonModel>.Filter.Eq(nameof(PersonModel.LastName), lastName);
        
        var findOptions = new FindOptions { Collation = new Collation("en", strength: CollationStrength.Primary) };

        List<PersonModel> items = await _mongoCollection
            .Find(filter, findOptions)
            .ToListAsync(cancellationToken);

        return items;
    }

    /// <inheritdoc/>
    public async Task<PersonModel?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/read-operations/retrieve/

        // If there are lots of items, see this example of walking through in batches:
        // https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/read-operations/retrieve/#example
        var filter = Builders<PersonModel>.Filter
            .And(Builders<PersonModel>.Filter.Eq(nameof(PersonModel.Id), id));

        var person = await _mongoCollection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return person;
    }

    /// <inheritdoc/>
    public async Task<ICollection<PersonModel>> GetAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PersonModel>.Filter.Empty;

        // https://stackoverflow.com/a/73262796/97803
        var sort = Builders<PersonModel>.Sort.Ascending(nameof(PersonModel.FirstName));

        var personList = await _mongoCollection
            .Find(filter)
            .Sort(sort)
            .Limit(limit)
            .ToListAsync(cancellationToken);

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

    /// <summary>
    /// Creates a FindOptions object that will make the search case-insensitive.
    /// </summary>
    /// <remarks>
    /// Note 1: A strength of 1 (primary) makes this a case-insensitive search. The documentation states:
    ///         "Primary level of comparison. Collation performs comparisons of the base characters only,
    ///          ignoring other differences such as diacritics and CASE."
    ///          See https://www.mongodb.com/docs/manual/reference/method/cursor.collation/
    /// Note 2:  Diacritics is a sign, such as an accent or cedilla, which when written above or below a
    ///          letter indicates a difference in pronunciation from the same letter when unmarked or differently marked
    /// </remarks>
    private static FindOptions CreateCaseInsensitiveFindOptions()
    {
        var result = new FindOptions
        {
            Collation = new Collation("en", strength: CollationStrength.Primary)
        };

        return result;
    }
}