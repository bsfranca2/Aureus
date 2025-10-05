using Aureus.Infrastructure.EntityFramework;
using Aureus.WebApi;
using Aureus.WebApi.Extensions;
using Aureus.WebApi.Middlewares;

using Carter;

using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddCarter()
    .AddEndpointsApiExplorer()
    .AddOpenApi()
    .AddDefaultCorsPolicy();

builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await databaseContext.Database.MigrateAsync();
    await databaseContext.Database.EnsureCreatedAsync();
}

app.UseCors();

app.UseForwardedHeaders();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseMiddleware<StoreTenantMiddleware>();

app.MapDefaultEndpoints();

app.MapCarter();

app.Run();