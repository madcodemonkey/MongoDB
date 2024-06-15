using WebApiCrudExample.Model;

namespace WebApiCrudExample.Data;

public interface IPersonRepository
{
    /// <summary>
    /// Adds a person to the repository.
    /// </summary>
    /// <param name="person">The person to add</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<PersonModel> AddAsync(PersonModel person, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a person from the repository.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a person exists in the repository.
    /// </summary>
    /// <param name="id">The primary key of the person to find.</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks to see if a person with the same first and last name exists in the repository.
    /// </summary>
    /// <param name="firstName">First name to check</param>
    /// <param name="lastName">Last name to check</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<bool> ExistsAsync(string firstName, string lastName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds all people with a given first and last name.
    /// </summary>
    /// <param name="firstName">First name to check</param>
    /// <param name="lastName">Last name to check</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<List<PersonModel>> FindByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a person from the repository.
    /// </summary>
    /// <param name="id">The primary key of the person to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<PersonModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all the people from the repository.  Sorted by first name.
    /// </summary>
    /// <param name="limit">The maximum number of items to return</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<ICollection<PersonModel>> GetAsync(int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a person in the repository.
    /// </summary>
    /// <param name="id">The primary key of the person to update.</param>
    /// <param name="person">New person</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<PersonModel> UpdateAsync(Guid id, PersonModel person, CancellationToken cancellationToken = default);
}