using Aureus.EventsProcessing;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.Run();