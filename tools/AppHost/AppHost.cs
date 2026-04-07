using AppHost.Commands;
using Azure.Provisioning;
using Azure.Provisioning.AppService;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Configure the Azure App Service environment
builder.AddAzureAppServiceEnvironment("plan").ConfigureInfrastructure(infra =>
{
    var plan = infra.GetProvisionableResources()
        .OfType<AppServicePlan>()
        .Single();

    plan.Sku = new AppServiceSkuDescription
    {
        Name = "B1", // Basic tier, 1 core
    };
});

var sqlServer = builder
    .AddAzureSqlServer("sql")
    .RunAsContainer(container =>
    {
        // Configure SQL Server to run locally as a container
        container.WithLifetime(ContainerLifetime.Persistent);

        // Use SQL Server 2022 as the default of SQL Server 2025 doesn't work on Linux/MacOS
        container.WithImage("mssql/server:2022-latest");

        // If desired, set SQL Server Port to a constant value
        //container.WithHostPort(1800);
    });

var db = sqlServer
    .AddDatabase("CleanArchitecture", "clean-architecture")
    .WithDropDatabaseCommand();

var migrationService = builder.AddProject<MigrationService>("migrations")
    .PublishAsAzureAppServiceWebsite((_, site) =>
    {
        const string envNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        // Needed for hosted service to run
        site.SiteConfig.IsAlwaysOn = true;

        // Dynamically set environment, so we can enable seeding of data (only happens in 'development')
        var environment = Environment.GetEnvironmentVariable(envNetCoreEnvironment);
        if (string.IsNullOrWhiteSpace(environment))
            return;

        var envSetting = new AppServiceNameValuePair { Name = envNetCoreEnvironment, Value = environment };
        site.SiteConfig.AppSettings.Add(new BicepValue<AppServiceNameValuePair>(envSetting));
    })
    .WithReference(db);

var api = builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithEnvironment("PORT", "8080");


//for now I just added the ConnectionString to the API, but ideally we should have the migration service run before the API and then the API can reference the migration service to get the connection string. This is because the migration service is responsible for creating the database and therefore should be the one to provide the connection string to the API.
//My Api does not hit a datbase yet. So I will just reference the db directly for now, but in the future we should consider having the migration service provide the connection string to the API. This way we can ensure that the database is created before the API tries to connect to it.
//.WithReference(db)
//.WaitForCompletion(migrationService)


var frontEnd = builder.AddProject<WebFrontEnd>("frontEnd")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);
//.WaitFor(sqlServer);

// Configure Application Insights and Log Analytics only if in publish mode
// When running locally, use Aspire Dashboard instead
if (builder.ExecutionContext.IsPublishMode)
{
    var logAnalytics = builder.AddAzureLogAnalyticsWorkspace("log-analytics");
    var insights = builder.AddAzureApplicationInsights("insights", logAnalytics);
    api.WithReference(insights);
    migrationService.WithReference(insights);
}
builder.Build().Run();