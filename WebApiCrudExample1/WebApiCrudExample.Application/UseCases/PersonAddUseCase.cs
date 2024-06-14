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
        // TODO: Add validation framework.
        
        PersonModel person = _mapper.Map<PersonModel>(request);
        await  _personRepository.AddAsync(person, cancellationToken);
        return _mapper.Map<PersonResponse>(person); ;
    }
}