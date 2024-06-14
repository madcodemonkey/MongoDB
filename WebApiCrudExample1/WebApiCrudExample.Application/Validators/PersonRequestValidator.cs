using FluentValidation;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Application;

public class PersonRequestValidator : AbstractValidator<PersonRequest>
{
    public PersonRequestValidator()
    {
        RuleFor(r => r.FirstName).NotEmpty();
        RuleFor(r => r.LastName).NotEmpty();
    }
}
