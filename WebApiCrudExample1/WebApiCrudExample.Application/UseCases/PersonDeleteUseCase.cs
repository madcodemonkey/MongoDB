using AutoMapper;
using WebApiCrudExample.Data;

namespace WebApiCrudExample.Application;

public class PersonDeleteUseCase
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    /// <summary>constructor</summary>
    public PersonDeleteUseCase(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _personRepository.DeleteAsync(id, cancellationToken);
    }
}