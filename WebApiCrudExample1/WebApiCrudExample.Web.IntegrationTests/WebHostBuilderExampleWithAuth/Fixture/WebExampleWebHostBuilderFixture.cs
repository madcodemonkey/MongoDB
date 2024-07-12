using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mongo2Go;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiCrudExample.Controllers;

namespace WebApiCrudExample.Web.IntegrationTests;

internal class WebExampleWebHostBuilderAuthFixture : IDisposable
{
    private const string MyIssuer = "my-issuer";
    private const string MyAudience = "my-audience";
    private const string MySuperSecretKey = "superSlkjilkjiohihihkhihiohiecretKey@345";
    private MongoDbRunner? _mongoDbRunner;

    public WebExampleWebHostBuilderAuthFixture()
    {
        _mongoDbRunner = MongoDbRunner.Start();

        var builder = new WebHostBuilder()
            .ConfigureServices((builder, services) =>
            {
                services.AddServices(builder.Configuration);

                // Add the controllers from the Web Project (not just the ones in this assembly).
                services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(PersonController).Assembly));

                services.AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(options =>
                {   
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = MyIssuer,
                        ValidAudience = MyAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(MySuperSecretKey))
                    };
                });
            })
            .ConfigureAppConfiguration((builder, config) =>
            {
                config.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new("DatabaseOptions:databaseName", "webapiexample2"),
                    new("DatabaseOptions:collectionName", "people2"),
                    new("DatabaseOptions:mongoDbConnectionString", _mongoDbRunner.ConnectionString),
                    new("EnableSwagger", "false")
                });
            })
            .Configure(app =>
            {
                app.UseMiddleware<ExceptionMiddleware>();

                app.UseRouting();
                
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints => endpoints.MapControllers());
            });

       Server = new TestServer(builder);
    }

    public HttpClient GetTestClient()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "John Doe"),
            new(ClaimTypes.Email, "john.doe@gmail.com"),
        };
        return GetTestClient(claims);
    }

    public HttpClient GetTestClient(List<Claim> claims)
    {
        var client = Server.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GenerateJwtToken(claims)}");
        return client;
    }

    public IServiceProvider Services => Server.Services;
    public TestServer Server { get; private set; }


    public string GenerateJwtToken(List<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(MySuperSecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var result = new JwtSecurityToken(
            issuer: MyIssuer,
            audience: MyAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(result);
    }

    public void Dispose()
    {
        if (_mongoDbRunner != null)
        {
            _mongoDbRunner.Dispose();
            _mongoDbRunner = null;
        }
        
        Server?.Dispose();
        Server = null!;
    }
}