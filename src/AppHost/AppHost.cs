using Projects;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> mainDbUsername = builder.AddParameter("postgres-username");
IResourceBuilder<ParameterResource> mainDbPassword = builder.AddParameter("postgres-password");
IResourceBuilder<PostgresServerResource> mainDb = builder
    .AddPostgres("postgres", mainDbUsername, mainDbPassword, 5432)
    .WithDataVolume();
IResourceBuilder<PostgresDatabaseResource> mainDbDatabase = mainDb.AddDatabase("Database", "AurumPayDev");

IResourceBuilder<ProjectResource> checkoutApi = builder
    .AddProject<WebApi>("CheckoutApi")
    .WithReference(mainDbDatabase)
    .WaitFor(mainDbDatabase);

builder.Build().Run();
