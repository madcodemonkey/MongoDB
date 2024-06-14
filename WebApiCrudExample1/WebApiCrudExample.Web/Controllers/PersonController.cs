using Microsoft.AspNetCore.Mvc;
using WebApiCrudExample.Application;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;

        /// <summary>Constructor</summary>
        public PersonController(ILogger<PersonController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson(PersonRequest person,
            [FromServices] PersonAddUseCase useCase, 
            CancellationToken cancellationToken)
        {
            var response = await useCase.AddAsync(person, cancellationToken);
            return Ok(response);
        }
    }
}
