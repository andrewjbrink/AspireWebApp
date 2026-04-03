namespace MudBlazorWebApp.Extensions;

// TODO: Can we remove this?
// #pragma warning disable IDE0055

public static class DependencyInjection
{
    public static void AddWebApiWeb(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
    }
}
// #pragma warning restore IDE0055
