using Aureus.Application;
using Aureus.Infrastructure;
using Aureus.Infrastructure.EntityFramework;
using Aureus.WebApi.Extensions;

using FluentValidation;

namespace Aureus.WebApi;

public static class DependencyConfig
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IHostEnvironment environment,
        IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.SetupEntityFramework();
        services.AddEfRepositories();
        // services.AddScoped<IDatabaseContext>(sp => sp.GetRequiredService<DatabaseContext>());

        services.AddHttpContextAccessor();

        services.AddPaymentServices(configuration);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ApplicationAssemblyReference).Assembly);
        });
        services.AddMediatRLoggingBehavior();
        services.AddMediatRFluentValidationBehavior();
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyReference).Assembly);

        return services;
    }
}