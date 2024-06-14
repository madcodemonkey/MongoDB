using AutoMapper;
using WebApiCrudExample.Data;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Application;

public class PersonAddUseCase
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    /// <summary>Constructor</summary>
    public PersonAddUseCase(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public async Task<PersonResponse> AddAsync(PersonRequest request, CancellationToken cancellationToken)
    {
        // ExceptionMiddleware.cs will intercept this exception to return a standard result
        if (await _personRepository.ExistsAsync(request.FirstName, request.LastName, cancellationToken))
        {
            throw new BadRequestUserException("Person already exists.", new Dictionary<string, string>
            {
                {nameof(PersonRequest.FirstName), "Person with first name and last name already exists."},
                {nameof(PersonRequest.LastName), "Person with first name and last name already exists."}
            }); 
        }

        PersonModel person = _mapper.Map<PersonModel>(request);
        await _personRepository.AddAsync(person, cancellationToken);
        return _mapper.Map<PersonResponse>(person); ;
    }
}