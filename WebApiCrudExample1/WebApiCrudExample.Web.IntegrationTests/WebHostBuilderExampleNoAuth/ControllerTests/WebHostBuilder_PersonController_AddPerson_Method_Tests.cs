using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Web.IntegrationTests;

[TestClass]
public class WebHostBuilder_PersonController_AddPerson_Method_Tests
{
    [TestMethod]
    public async Task AddPerson_UserIsAdded_WhenAddIsCalled()
    {
        // Arrange
        using var fixture = new WebExampleWebHostBuilderFixture();

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 34
        });

        var client = fixture.GetTestClient();

        // Act
        var result = await client.PostAsync("/person", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await result.Content.ReadFromJsonAsync<PersonResponse>();
        personResponse.Should().NotBeNull();
        personResponse.Age.Should().Be(34);
        personResponse.FirstName.Should().Be("John");
        personResponse.LastName.Should().Be("Doe");

        // Make sure the person was added to the database
        var mongoCollection = fixture.Services.GetRequiredService<IMongoCollection<PersonModel>>();
        var person = await mongoCollection.Find(x => x.FirstName == "John" && x.LastName == "Doe").FirstOrDefaultAsync();
        person.Should().NotBeNull();
        person.FirstName.Should().Be("John");
        person.LastName.Should().Be("Doe");
    }

    [TestMethod]
    public async Task AddPerson_AddingPersonWithSameFirstAndLastName_ResultsInBadRequest()
    {
        // Arrange
        using var fixture = new WebExampleWebHostBuilderFixture();

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 34
        });

        var client = fixture.GetTestClient();


        // Adding the person the first time should be successful
        var resultFirstTime = await client.PostAsync("/person", context);
        resultFirstTime.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var resultSecondTime = await client.PostAsync("/person", context);

        // Assert
        resultSecondTime.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationDetails = await resultSecondTime.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        validationDetails.Should().NotBeNull();
        validationDetails.Errors.Should().ContainKey(nameof(PersonRequest.FirstName));
        validationDetails.Errors.Should().ContainKey(nameof(PersonRequest.LastName));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task AddPerson_NotSpecifyingFirstName_ResultsInBadRequest(string firstName)
    {
        // Arrange
        using var fixture = new WebExampleWebHostBuilderFixture();

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = firstName,
            LastName = "Doe",
            Age = 34
        });

        var client = fixture.GetTestClient();


        // Act
        var result = await client.PostAsync("/person", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationDetails = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        validationDetails.Should().NotBeNull();
        validationDetails.Errors.Should().ContainKey(nameof(PersonRequest.FirstName));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task AddPerson_NotSpecifyingLastName_ResultsInBadRequest(string lastName)
    {
        // Arrange
        using var fixture = new WebExampleWebHostBuilderFixture();

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = "John",
            LastName = lastName,
            Age = 34
        });

        var client = fixture.GetTestClient();


        // Act
        var result = await client.PostAsync("/person", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationDetails = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        validationDetails.Should().NotBeNull();
        validationDetails.Errors.Should().ContainKey(nameof(PersonRequest.LastName));

    }
}