using AutoMapper;
using WebApiCrudExample.Data;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Application;

public class PersonReplaceUseCase
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    /// <summary>constructor</summary>
    public PersonReplaceUseCase(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public async Task<PersonResponse> ReplaceAsync(Guid id, PersonRequest request, CancellationToken cancellationToken)
    {
        var existingPerson = await _personRepository.GetAsync(id, cancellationToken);
        if (existingPerson == null)
        {
            throw new NotFoundUserException("Person not found.", new Dictionary<string, string>
            {
                { nameof(PersonModel.Id), $"No person with id {id} was found." }
            });
        }

        if (existingPerson.FirstName != request.FirstName || existingPerson.LastName != request.LastName)
        {
            if (await _personRepository.ExistsAsync(request.FirstName, request.LastName, cancellationToken))
            {
                throw new BadRequestUserException("Person already exists.", new Dictionary<string, string>
                {
                    {nameof(PersonRequest.FirstName), "Person with first name and last name already exists."},
                    {nameof(PersonRequest.LastName), "Person with first name and last name already exists."}
                });
            }
        }
        

        PersonModel person = _mapper.Map<PersonModel>(request);
        return _mapper.Map<PersonResponse>(await _personRepository.UpdateAsync(id, person, cancellationToken));
    }
}

