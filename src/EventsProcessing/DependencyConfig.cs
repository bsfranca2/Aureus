using Aureus.Application;
using Aureus.Domain.Payments.Events;
using Aureus.Infrastructure;
using Aureus.Infrastructure.EntityFramework;

using Bsfranca2.Messaging.Configurations;
using Bsfranca2.Messaging.Extensions;

namespace Aureus.EventsProcessing;

public static class DependencyConfig
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services.SetupEntityFramework();
        services.AddEfRepositories();

        services.AddServices(configuration);

        services.AddEventHandlersFromAssemblies(typeof(ApplicationAssemblyReference).Assembly);

        services.AddMessaging(context =>
        {
            context.WithConsumer().UseDirectMessageProcessor();
            string connectionString = configuration.GetConnectionString("Messaging")
                                      ?? "amqp://guest:guest@localhost:5672/";
            context.UsingRabbitMq(connectionString, rabbit =>
            {
                rabbit.WithInfrastructureSetup();
                
                rabbit.ConfigureTopology(topology =>
                {
                    topology
                        .AddExchange(new ExchangeConfiguration { Name = "payments", Type = ExchangeType.Topic })
                        .AddQueue(new QueueConfiguration
                        {
                            Name = "process-payments",
                            ExchangeName = "payments",
                            RoutingKey = "payment.created",
                            PrefetchCount = 10
                        })
                        .MapEvent<ProcessPaymentRequestedEvent>("payments", "payment.created");
                });
            });
        });;

        return services;
    }
}