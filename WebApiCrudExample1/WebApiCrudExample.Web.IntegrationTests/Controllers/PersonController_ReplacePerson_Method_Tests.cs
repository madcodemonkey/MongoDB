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



    [TestMethod]
    public async Task ReplacePerson_BadRequestIsReturned_WhenTryingToChangePersonNameToAnExistingPersonsName()
    {
        // Arrange
        using var fixture = new WebExampleApplicationFixture();

        var existingPerson1 = await CreateExistingPersonAsync(fixture, "John", "Doe", 34);
        var existingPerson2 = await CreateExistingPersonAsync(fixture, "Jane", "Doe", 32);


        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = existingPerson1.FirstName,
            LastName = existingPerson1.LastName,
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
 
}