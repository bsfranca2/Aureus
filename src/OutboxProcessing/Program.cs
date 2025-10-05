using Aureus.OutboxProcessing;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);

builder.Services.AddHostedService<OutboxBackgroundService>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.Run();