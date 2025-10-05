using Aureus.Application;
using Aureus.Domain.Payments.Events;
using Aureus.Infrastructure;
using Aureus.Infrastructure.EntityFramework;

using Bsfranca2.Messaging.Configurations;
using Bsfranca2.Messaging.Extensions;

using Npgsql;

namespace Aureus.OutboxProcessing;

public static class DependencyConfig
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services.AddSingleton<NpgsqlDataSource>(_ =>
        {
            string connectionString = configuration.GetConnectionString("Database") ??
                                      throw new InvalidOperationException("Connection string 'Database' not found.");

            NpgsqlDataSourceBuilder dataSourceBuilder = new(connectionString);
            return dataSourceBuilder.Build();
        });

        services.AddMessaging(context =>
        {
            string connectionString = configuration.GetConnectionString("Messaging")
                                      ?? "amqp://guest:guest@localhost:5672/";
            context.UsingRabbitMq(connectionString, rabbit =>
            {
                rabbit.ConfigureTopology(topology =>
                {
                    topology
                        .AddExchange(new ExchangeConfiguration { Name = "payments", Type = ExchangeType.Topic })
                        .MapEvent<ProcessPaymentRequestedEvent>("payments", "payment.created");
                });
            });
        });
        
        services.AddScoped<OutboxProcessor>();

        return services;
    }
}