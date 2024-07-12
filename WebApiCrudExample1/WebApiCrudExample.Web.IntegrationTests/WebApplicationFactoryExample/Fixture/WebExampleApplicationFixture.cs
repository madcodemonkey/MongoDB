namespace WebApiCrudExample.Web.IntegrationTests;

public class WebExampleApplicationFixture : IDisposable
{
    public WebExampleApplicationFactory WebExampleApplicationFactory { get; }

    public HttpClient TestHttpClient { get; private set; }

    public IServiceProvider TestServiceProvider { get; private set; }

    public WebExampleApplicationFixture()
    {
        WebExampleApplicationFactory = new WebExampleApplicationFactory();
        
        TestHttpClient = WebExampleApplicationFactory.CreateDefaultClient();
        
        // Add default headers?
        
        TestServiceProvider = WebExampleApplicationFactory.Services;

    }

    public void Dispose()
    {
        WebExampleApplicationFactory.Dispose();
    }
}