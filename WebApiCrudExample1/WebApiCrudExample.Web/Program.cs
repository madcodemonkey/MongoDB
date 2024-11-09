using Microsoft.AspNetCore.Http.Features;
using WebApiCrudExample.Data.Extensions;
using WebApiCrudExample.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

            var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
        };
    }
);
builder.Services.AddExceptionHandler<MyCustomExceptionHandler>();

var app = builder.Build();

// Used for creating indexes and other MongoDB specific changes
await app.Services.ApplyMongoDbChangesAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }
