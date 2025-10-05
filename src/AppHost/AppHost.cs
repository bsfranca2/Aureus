using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> mainDbUsername = builder.AddParameter("postgres-username");
IResourceBuilder<ParameterResource> mainDbPassword = builder.AddParameter("postgres-password");
IResourceBuilder<PostgresServerResource> mainDb = builder
    .AddPostgres("postgres", mainDbUsername, mainDbPassword, 5432)
    .WithDataVolume();
IResourceBuilder<PostgresDatabaseResource> mainDbDatabase = mainDb.AddDatabase("Database", "AureusDev");

IResourceBuilder<ParameterResource> messagingUsername = builder.AddParameter("rabbitmq-username");
IResourceBuilder<ParameterResource> messagingPassword = builder.AddParameter("rabbitmq-password");
IResourceBuilder<RabbitMQServerResource> messaging = builder
    .AddRabbitMQ("Messaging", messagingUsername, messagingPassword)
    .WithManagementPlugin()
    .WithDataVolume(isReadOnly: false);

builder
    .AddProject<WebApi>("CheckoutApi")
    .WithReference(mainDbDatabase)
    .WaitFor(mainDbDatabase);

builder
    .AddProject<OutboxProcessing>("OutboxProcessing")
    .WithReference(mainDbDatabase)
    .WaitFor(mainDbDatabase)
    .WithReference(messaging)
    .WaitFor(messaging);

builder
    .AddProject<EventsProcessing>("EventsProcessing")
    .WithReference(mainDbDatabase)
    .WaitFor(mainDbDatabase)
    .WithReference(messaging)
    .WaitFor(messaging);

builder.Build().Run();