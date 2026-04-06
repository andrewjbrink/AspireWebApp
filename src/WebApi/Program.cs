using AspireWebApp.Application;
using AspireWebApp.Infrastructure;
using AspireWebApp.WebApi;
using AspireWebApp.WebApi.Endpoints;
using AspireWebApp.WebApi.Extensions;
using AspireWebApp.WebApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddCustomProblemDetails();

builder.Services.AddWebApi(builder.Configuration);
builder.Services.AddApplication();
builder.AddInfrastructure();
builder.AddCustomExtensions();
builder.Services.AddSignalR();

var app = builder.Build();

//app.MapHub<SalesHub>("/salesHub");



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.MapOpenApi();
app.MapCustomScalarApiReference();
app.UseHealthChecks();
app.UseHttpsRedirection();
app.UseStaticFiles();

//app.MapHeroEndpoints();
app.MapValuationEndpoints();
//app.MapTeamEndpoints();
app.UseEventualConsistencyMiddleware();

app.ApplyApiCorsConfig();


app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.Run();