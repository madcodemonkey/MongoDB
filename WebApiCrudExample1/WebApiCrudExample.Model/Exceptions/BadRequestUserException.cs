using System.Net;

namespace WebApiCrudExample.Model;

public class BadRequestUserException : UserException
{
    public BadRequestUserException(string message) : base("Bad Request", message) { }

    public BadRequestUserException(string message, Dictionary<string, string> errors) : base("Bad Request", message, errors) { }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public override string ErrorType => "https://tools.ietf.org/html/rfc9110#section-15.5.1";
}