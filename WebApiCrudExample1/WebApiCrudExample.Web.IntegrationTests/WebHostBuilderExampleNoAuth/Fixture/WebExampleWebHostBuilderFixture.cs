using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using WebApiCrudExample.Controllers;

namespace WebApiCrudExample.Web.IntegrationTests;

internal class WebExampleWebHostBuilderFixture : IDisposable
{
    public TestServer Server { get; private set; }
    private MongoDbRunner? _mongoDbRunner;
  
    public WebExampleWebHostBuilderFixture()
    {
        _mongoDbRunner = MongoDbRunner.Start();

        var builder = new WebHostBuilder()
            .ConfigureServices((builder, services) =>
            {
                services.AddServices(builder.Configuration);

                // Add the controllers from the Web Project (not just the ones in this assembly).
                services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(PersonController).Assembly));
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

                app.UseEndpoints(endpoints => endpoints.MapControllers());
            });

       Server = new TestServer(builder);
    }

    public HttpClient GetTestClient() => Server.CreateClient();
    public IServiceProvider Services => Server.Services;
    
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