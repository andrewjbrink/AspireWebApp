using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;

namespace AspireWebApp.WebApi.Extensions;

public static class ConfigureExtensions
{
    private const string CorsPolicyName = "AllowAllOrigins";
    public static void AddCustomExtensions(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        //Observe camel case for Json
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = null;
            options.PropertyNameCaseInsensitive = true;

        });

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
        });

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName,
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

    }

    public static void ApplyApiCorsConfig(this WebApplication app)
    {
        app.UseCors(CorsPolicyName);
        app.UseCors("AllowBlazor");
    }
}
