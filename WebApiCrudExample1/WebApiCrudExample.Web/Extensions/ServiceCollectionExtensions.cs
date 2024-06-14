using WebApiCrudExample.Application;
using WebApiCrudExample.Data;

namespace WebApiCrudExample.Web;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        //// Options pattern: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#the-options-pattern
        //// Register it with DI via configure options: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#use-di-services-to-configure-options
        //services.Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.SectionName));

        //services.AddTransient<IPersonRepository, PersonRepository>();

        services.AddDataServices(config);
        services.AddApplicationServices(config);

        return services;
    }
}