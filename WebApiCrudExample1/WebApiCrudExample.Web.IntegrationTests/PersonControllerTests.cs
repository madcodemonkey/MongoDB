using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Web.IntegrationTests;

[TestClass]
public class PersonControllerTests
{
    [TestMethod]
    public async Task AddPerson_UserIsAdded_WhenAddIsCalled()
    {
        // Arrange
        using var fixture = new WebExampleApplicationFixture();

        var context = JsonContent.Create(new PersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 34
        });

        // Act
        var result = await fixture.TestHttpClient.PostAsync("/person", context);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await result.Content.ReadFromJsonAsync<PersonResponse>();
        personResponse.Should().NotBeNull();
        personResponse.Age.Should().Be(34);
        personResponse.FirstName.Should().Be("John");
        personResponse.LastName.Should().Be("Doe");
    }
}