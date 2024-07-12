using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiCrudExample.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MySecureController : ControllerBase
    {
        [HttpGet("values")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetClaimValues(CancellationToken cancellationToken)
        {
            var list =  User.Claims.ToList().Select(claim => claim.Value).ToList();
            return Ok(list);
        }
    }
}
