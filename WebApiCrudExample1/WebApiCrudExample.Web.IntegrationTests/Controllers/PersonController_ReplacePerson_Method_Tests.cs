using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Web.IntegrationTests;

[TestClass]
public class PersonController_ReplacePerson_Method_Tests
{
    [TestMethod]
    public async Task ReplacePerson_UserIsUpdated_WhenAgeIsChanged()
    {
        // Arrange
        using var fixture = new WebExampleApplicationFixture();

        var existingPerson = await CreateExistingPersonAsync(fixture, "John", "Doe", 34);

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = existingPerson.FirstName,
            LastName = existingPerson.LastName,
            Age = 42
        });

        // Act
        var result = await fixture.TestHttpClient.PutAsync($"/person/{existingPerson.Id}", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await result.Content.ReadFromJsonAsync<PersonResponse>();
        personResponse.Should().NotBeNull();
        personResponse.Age.Should().Be(42); 

        // Make sure the person was added to the database
        var mongoCollection = fixture.TestServiceProvider.GetRequiredService<IMongoCollection<PersonModel>>();
        var person = await mongoCollection.Find(x => x.FirstName == "John" && x.LastName == "Doe").FirstOrDefaultAsync();
        person.Should().NotBeNull();
        person.Age.Should().Be(42);
    }

    /// <summary>
    /// Name is supposed to be unique, but we are allowed to update the name of the current person.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    [DataRow("john", "doe", "John", "Doe")]
    [DataRow("john", "doe", "John", "doe")]
    [DataRow("john", "doe", "john", "Doe")]
    [DataRow("john", "doe", "JOHN", "DOE")]
    public async Task ReplacePerson_UserIsUpdated_WhenNameIsChanged(string currentFirstName, string currentLastName, 
        string newFirstName, string newLastName)
    {
        // Arrange
        using var fixture = new WebExampleApplicationFixture();

        var existingPerson = await CreateExistingPersonAsync(fixture, currentFirstName, currentLastName, 42);

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = newFirstName,
            LastName = newLastName,
            Age = 42
        });

        // Act
        var result = await fixture.TestHttpClient.PutAsync($"/person/{existingPerson.Id}", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await result.Content.ReadFromJsonAsync<PersonResponse>();
        personResponse.Should().NotBeNull();
        personResponse.FirstName.Should().Be(newFirstName);
        personResponse.LastName.Should().Be(newLastName);

        // Make sure the person was added to the database
        var mongoCollection = fixture.TestServiceProvider.GetRequiredService<IMongoCollection<PersonModel>>();
        
        // Note: This is a case-sensitive search!
        var person = await mongoCollection
            .Find(x => x.FirstName == newFirstName && x.LastName == newLastName)
            .FirstOrDefaultAsync();

        person.Should().NotBeNull();
        person.FirstName.Should().Be(newFirstName);
        person.LastName.Should().Be(newLastName);
    }




    [TestMethod]
    [DataRow("john", "doe", "jane", "doe", "john", "doe")]  // cases are all the same
    [DataRow("John", "Doe", "Jane", "Doe", "John", "Doe")]  // cases are all the same
    [DataRow("John", "Doe", "Jane", "Doe", "john", "Doe")]  // case of new first name is different 
    [DataRow("John", "Doe", "Jane", "Doe", "John", "doe")]  // case of new last name is different 
    public async Task ReplacePerson_BadRequestIsReturned_WhenTryingToChangePersonNameToAnExistingPersonsName(
        string person1FirstName, string person1LastName,
        string person2FirstName, string person2LastName,
        string person2NewFirstName, string person2NewLastName)
    {
        // Arrange
        using var fixture = new WebExampleApplicationFixture();

        var existingPerson1 = await CreateExistingPersonAsync(fixture, person1FirstName, person1LastName, 34);
        var existingPerson2 = await CreateExistingPersonAsync(fixture, person2FirstName, person2LastName, 32);


        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = person2NewFirstName,
            LastName = person2NewLastName,
            Age = existingPerson2.Age
        });
        
        // Act
        var result = await fixture.TestHttpClient.PutAsync($"/person/{existingPerson2.Id}", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationDetails = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        validationDetails.Should().NotBeNull();
        validationDetails.Errors.Should().ContainKey(nameof(PersonRequest.FirstName));
        validationDetails.Errors.Should().ContainKey(nameof(PersonRequest.LastName));
    }


    /// <summary>
    /// Creates a person in the database so that we can test replacing it in multiple tests.
    /// </summary>
    private async Task<PersonResponse> CreateExistingPersonAsync(WebExampleApplicationFixture fixture, string firstName, string lastName, int age)
    {
        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Age = age
        });

        var result = await fixture.TestHttpClient.PostAsync("/person", context);
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await result.Content.ReadFromJsonAsync<PersonResponse>();
        personResponse.Should().NotBeNull();
        return personResponse;
    }

}