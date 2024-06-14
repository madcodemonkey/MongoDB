using WebApiCrudExample.Model;

namespace WebApiCrudExample.Application.UnitTests.Validators;

[TestClass]
public class PersonRequestValidatorTests
{
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void PersonRequestValidator_WhenFirstNameIsEmpty_ShouldHaveError(string firstName)
    {
        // Arrange
        var personRequest = new PersonRequest
        {
            FirstName = firstName,
            LastName = "Doe",
            Age = 34
        };

        var validator = new PersonRequestValidator();

        // Act
        var result = validator.Validate(personRequest);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(PersonRequest.FirstName)));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void PersonRequestValidator_WhenLastNameIsEmpty_ShouldHaveError(string lastName)
    {
        // Arrange
        var personRequest = new PersonRequest
        {
            FirstName = "John",
            LastName = lastName,
            Age = 34
        };

        var validator = new PersonRequestValidator();

        // Act
        var result = validator.Validate(personRequest);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(PersonRequest.LastName)));
    }
}