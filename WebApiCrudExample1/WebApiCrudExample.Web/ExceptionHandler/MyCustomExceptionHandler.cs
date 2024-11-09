using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Web;

public class MyCustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<MyCustomExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    /// <summary>
    /// Constructor
    /// </summary>
    public MyCustomExceptionHandler(ILogger<MyCustomExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogError(ex, ex.Message);

        if (ex is not UserException userException)
            return false;

        ProblemDetails problemDetails;

        if (userException.Errors.Count > 0)
        {
            problemDetails = new ValidationProblemDetails
            {
                Type = userException.ErrorType,
                Title = userException.Title,
                Detail = userException.Message,
                Status = (int)userException.StatusCode,
                Errors = new Dictionary<string, string[]>()
            };

            foreach (var error in userException.Errors)
            {
                (problemDetails as ValidationProblemDetails)?.Errors.Add(error.Key, new[] { error.Value });
            }
        }
        else
        {
            problemDetails = new ProblemDetails
            {
                Type = userException.ErrorType,
                Title = userException.Title,
                Detail = userException.Message,
                Status = (int)userException.StatusCode
            };
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}