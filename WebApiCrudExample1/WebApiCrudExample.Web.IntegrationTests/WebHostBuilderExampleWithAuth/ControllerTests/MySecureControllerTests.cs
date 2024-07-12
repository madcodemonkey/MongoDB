using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;

namespace WebApiCrudExample.Web.IntegrationTests;

[TestClass]
public class MySecureControllerTests
{
    [TestMethod]
    public async Task GetClaimValues_CanRetrieveValues()
    {
        // Arrange
        using var fixture = new WebExampleWebHostBuilderAuthFixture();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Mick Jagger"),
            new(ClaimTypes.Email, "mick.jagger@gmail.com"),
        };
        var client = fixture.GetTestClient(claims);
         
        // Act
        var result = await client.GetAsync("/MySecure/Values");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var claimList = await result.Content.ReadFromJsonAsync<List<string>>();
        claimList.Should().ContainMatch("Mick Jagger");
        claimList.Should().ContainMatch("mick.jagger@gmail.com");
    }
}