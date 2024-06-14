using System.Net;

namespace WebApiCrudExample.Model;

public class NotFoundUserException : UserException
{
    public NotFoundUserException(string title) : base(title) { }

    public NotFoundUserException(string title, Dictionary<string, string> errors) : base(title, errors) { }

    
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public override string ErrorType => "https://tools.ietf.org/html/rfc9110#section-15.5.5";
}