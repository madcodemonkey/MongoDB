using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Mongo2Go;

namespace WebApiCrudExample.Web.IntegrationTests;

public class WebExampleApplicationFactory : WebApplicationFactory<Program>
{
    private MongoDbRunner? _mongoDbRunner;

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
                new("hostBuilder:reloadConfigOnChange", "false"),
            });

        })
        .ConfigureAppConfiguration((hostingContext, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
            {
                new("DatabaseOptions:mongoDbConnectionString", _mongoDbRunner.ConnectionString),
                new("EnableSwagger", "false")
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
        if (_mongoDbRunner != null)
        {
            _mongoDbRunner.Dispose();
            _mongoDbRunner = null;
        }
        base.Dispose(disposing);
    }
}