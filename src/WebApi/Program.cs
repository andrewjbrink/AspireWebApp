using AspireWebApp.Application;
using AspireWebApp.Infrastructure;
using AspireWebApp.WebApi;
using AspireWebApp.WebApi.Endpoints;
using AspireWebApp.WebApi.Extensions;
using AspireWebApp.WebApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Bind to Azure App Service port if provided
var port = Environment.GetEnvironmentVariable("PORT");
var runningInAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));

if (runningInAzure && !string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}


builder.AddServiceDefaults();
builder.Services.AddCustomProblemDetails();

builder.Services.AddWebApi(builder.Configuration);
builder.Services.AddApplication();
builder.AddInfrastructure();
builder.AddCustomExtensions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.MapOpenApi();
app.MapCustomScalarApiReference();
app.UseHealthChecks();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapValuationEndpoints();
app.UseEventualConsistencyMiddleware();

app.ApplyApiCorsConfig();

app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.Run();
