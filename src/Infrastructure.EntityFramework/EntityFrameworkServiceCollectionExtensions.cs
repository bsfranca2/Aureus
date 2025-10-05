using Aureus.Domain.Shared.Interfaces;
using Aureus.Infrastructure.EntityFramework.Options;
using Aureus.Infrastructure.EntityFramework.Repositories;
using Aureus.Infrastructure.EntityFramework.ValueGenerators;

using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aureus.Infrastructure.EntityFramework;

public static class EntityFrameworkServiceCollectionExtensions
{
    public static IServiceCollection SetupEntityFramework(this IServiceCollection services)
    {
        services.ConfigureOptions<DatabaseOptionsSetup>();

        services.AddDbContext<DatabaseContext>(ConfigureDbContextOptions);

        return services;
    }

    private static void ConfigureDbContextOptions(IServiceProvider serviceProvider,
        DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        DatabaseOptions databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        dbContextOptionsBuilder.UseNpgsql(databaseOptions.ConnectionString, npgsqlDbContextOptionsBuilder =>
        {
            // npgsqlDbContextOptionsBuilder.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
            npgsqlDbContextOptionsBuilder.CommandTimeout(databaseOptions.CommandTimeout);
        }).UseAsyncSeeding(async (context, _, ct) => await DatabaseContextSeed.Seed((DatabaseContext)context, ct));

        dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
        dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
    }

    public static IServiceCollection AddEfRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IPaymentGatewayRepository, PaymentGatewayRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        return services;
    }
}