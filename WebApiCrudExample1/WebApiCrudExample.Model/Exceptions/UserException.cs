using System.Net;

namespace WebApiCrudExample.Model;

public abstract class UserException : Exception
{
    protected UserException(string title, string message) : base(message)
    {
        Title = title;
        Errors = new Dictionary<string, string>();
    }

    protected UserException(string title, string message, Dictionary<string, string> errors) : base(message)
    {
        Title = title;
        Errors = errors;
    }

    public abstract HttpStatusCode StatusCode { get; }

    public abstract string ErrorType { get; }

    public Dictionary<string, string> Errors { get; set; }

    public string Title { get; set; }

    public void AddError(string key, string value)
    {
        Errors.Add(key, value);
    }
}