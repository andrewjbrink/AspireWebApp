using AspireWebApp.Infrastructure;
using MudBlazor.Services;
using MudBlazorWebApp.Components;
using MudBlazorWebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration["services:api:https:0"]; //services:api:https:0


builder.AddServiceDefaults();
builder.Services.AddWebApiWeb();
builder.AddInfrastructureWeb();

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddScoped<JavaHelper>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddHttpClient("https+http://apiservice");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl!)
});


var app = builder.Build();




InteropLogger.Configure(app.Services.GetRequiredService<ILoggerFactory>());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapGet("/config.js", async context =>
{
    context.Response.ContentType = "application/javascript";
    await context.Response.WriteAsync($"window.__config = {{ apiBaseUrl: '{apiBaseUrl}' }};");
});



app.Run();
