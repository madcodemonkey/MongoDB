﻿using WebApiCrudExample.Model;

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
    /// Gets a person from the repository.
    /// </summary>
    /// <param name="id">The primary key of the person to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<PersonModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a person exists in the repository.
    /// </summary>
    /// <param name="id">The primary key of the person to find.</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all the people from the repository.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<ICollection<PersonModel>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a person in the repository.
    /// </summary>
    /// <param name="id">The primary key of the person to update.</param>
    /// <param name="person">New person</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<PersonModel> UpdateAsync(Guid id, PersonModel person, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a person from the repository.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}