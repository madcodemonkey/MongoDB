using System.Net;

namespace WebApiCrudExample.Model;

public abstract class UserException : Exception
{
    protected UserException(string message) : base(message)
    {
        Errors = new Dictionary<string, string>();
    }

    protected UserException(string title, Dictionary<string, string> errors) : base(title)
    {
        Errors = errors;
    }

    public abstract HttpStatusCode StatusCode { get; }

    public abstract string ErrorType { get; }
 
    public Dictionary<string, string> Errors { get; set; } 

    public void AddError(string key, string value)
    {
        Errors.Add(key, value);
    }
}