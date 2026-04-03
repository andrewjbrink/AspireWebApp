using AppHost.Commands;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("aspire-docker-demo");

// Configure the Azure App Service environment
//builder.AddAzureAppServiceEnvironment("plan").ConfigureInfrastructure(infra =>
//{
//    var plan = infra.GetProvisionableResources()
//        .OfType<AppServicePlan>()
//        .Single();

//    plan.Sku = new AppServiceSkuDescription
//    {
//        Name = "B1", // Basic tier, 1 core
//    };
//});


//I removed this
//.AddAzureSqlServer("sql")

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
    .WithReference(db)
    .WaitFor(sqlServer);

var api = builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WaitForCompletion(migrationService);

var frontEnd = builder.AddProject<MudBlazorWebApp>("frontEnd")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api)
    .WaitFor(sqlServer);

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