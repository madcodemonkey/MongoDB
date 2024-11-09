using System.Net;

namespace WebApiCrudExample.Model;

public class NotFoundUserException : UserException
{
    public NotFoundUserException(string message) : base("Not Found", message) { }

    public NotFoundUserException(string message, Dictionary<string, string> errors) : base("Not Found", message, errors) { }


    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public override string ErrorType => "https://tools.ietf.org/html/rfc9110#section-15.5.5";
}