using System.Net;

namespace WebApiCrudExample.Model;

public class BadRequestUserException : UserException
{
    public BadRequestUserException(string title) : base(title) { }

    public BadRequestUserException(string title, Dictionary<string, string> errors) : base(title, errors) { }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public override string ErrorType => "https://tools.ietf.org/html/rfc9110#section-15.5.1";
}