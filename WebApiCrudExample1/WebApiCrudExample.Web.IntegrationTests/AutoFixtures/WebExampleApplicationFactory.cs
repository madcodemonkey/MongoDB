using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo2Go;

namespace WebApiCrudExample.Web.IntegrationTests;

public class WebExampleApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MongoDbRunner _mongoDbRunner;

    public WebExampleApplicationFactory()
    {
        _mongoDbRunner = MongoDbRunner.Start();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                // https://stackoverflow.com/a/72031794/97803
                new KeyValuePair<string, string?>("hostBuilder:reloadConfigOnChange", "false"),
            });

        })
        .ConfigureAppConfiguration((hostingContext, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                new KeyValuePair<string, string?>("DatabaseOptions:mongoDbConnectionString", _mongoDbRunner.ConnectionString),
            });

        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");
        builder.ConfigureTestServices(services =>
        {

        });
    }

    protected override void Dispose(bool disposing)
    {
        _mongoDbRunner.Dispose();
        base.Dispose(disposing);
    }
}