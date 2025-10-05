using Aureus.Domain.Configuration;
using Aureus.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aureus.Infrastructure;

public static class DependencyConfig
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
        services.AddScoped<IStorePaymentConfigurationService, StorePaymentConfigurationService>();

        // services.AddScoped<IPaymentGatewayService, MercadoPagoGatewayService>();

        return services;
    }
}