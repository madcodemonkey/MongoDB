using Microsoft.AspNetCore.Mvc;
using WebApiCrudExample.Application;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPerson(PersonRequest person,
            [FromServices] PersonAddUseCase useCase, 
            CancellationToken cancellationToken)
        {
            var response = await useCase.AddAsync(person, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePerson(Guid id,
            [FromServices] PersonDeleteUseCase useCase,
            CancellationToken cancellationToken)
        {
            await useCase.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPeople(
            [FromServices] PersonGetAllUseCase useCase,
            CancellationToken cancellationToken)
        {
            var response = await useCase.GetAllAsync(limit: 100, cancellationToken);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplacePerson(Guid id, PersonRequest person,
            [FromServices] PersonReplaceUseCase useCase,
            CancellationToken cancellationToken)
        {
            var response = await useCase.ReplaceAsync(id, person, cancellationToken);
            return Ok(response);
        }

 
    }
}
