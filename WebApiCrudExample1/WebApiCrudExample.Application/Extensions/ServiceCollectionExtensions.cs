using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiCrudExample.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(typeof(MappingProfiles));

        // Options pattern: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#the-options-pattern
        // Register it with DI via configure options: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0#use-di-services-to-configure-options
        services.Configure<ApplicationOptions>(config.GetSection(ApplicationOptions.SectionName));

       // Use cases
       services.AddScoped<PersonAddUseCase>();

        return services;
    }
}