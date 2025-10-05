using Aureus.Domain.Configuration;
using Aureus.Domain.Shared.Interfaces;
using Aureus.Infrastructure.PaymentGateways.MercadoPago;
using Aureus.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aureus.Infrastructure;

public static class DependencyConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        
        services.AddScoped<IWorkContext, WorkContext>();
        
        services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
        services.AddScoped<IStorePaymentConfigurationService, StorePaymentConfigurationService>();

        services.AddScoped<IPaymentGatewayService, MercadoPagoGatewayService>();

        return services;
    }
}