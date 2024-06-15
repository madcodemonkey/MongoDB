using AutoMapper;
using WebApiCrudExample.Data;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Application;

public class PersonGetAllUseCase
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    /// <summary>constructor</summary>
    public PersonGetAllUseCase(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PersonResponse>> GetAllAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        var people = await _personRepository.GetAsync(limit, cancellationToken);
        return _mapper.Map<IEnumerable<PersonResponse>>(people);
    }
}