using Aureus.Domain.Configuration;
using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Stores;
using Aureus.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Aureus.Infrastructure.Services;

public class StorePaymentConfigurationService(DatabaseContext dbContext) : IStorePaymentConfigurationService
{
    public async Task<StorePaymentConfiguration?> GetActiveConfigurationAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StorePaymentConfigurations
            .FirstOrDefaultAsync(spc => spc.StoreId == storeId
                                        && spc.PaymentMethodId == paymentMethodId
                                        && spc.IsEnabled
                                        && spc.IsActive,
                cancellationToken);
    }

    public async Task<IEnumerable<StorePaymentConfiguration>> GetAllConfigurationsAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StorePaymentConfigurations
            .Where(spc => spc.StoreId == storeId && spc.PaymentMethodId == paymentMethodId)
            .ToListAsync(cancellationToken);
    }

    public async Task<StorePaymentConfiguration?> GetConfigurationAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId gatewayId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StorePaymentConfigurations
            .FirstOrDefaultAsync(spc => spc.StoreId == storeId
                                        && spc.PaymentMethodId == paymentMethodId
                                        && spc.PaymentGatewayId == gatewayId,
                cancellationToken);
    }

    public async Task SetActiveGatewayAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId gatewayId,
        CancellationToken cancellationToken = default)
    {
        // Get all configurations for this store and payment method
        IEnumerable<StorePaymentConfiguration> configurations =
            await GetAllConfigurationsAsync(storeId, paymentMethodId, cancellationToken);

        // Deactivate all configurations
        foreach (StorePaymentConfiguration config in configurations)
        {
            config.Deactivate();
        }

        // Activate the selected gateway
        StorePaymentConfiguration? targetConfig = configurations.FirstOrDefault(c => c.PaymentGatewayId == gatewayId);
        if (targetConfig is not null)
        {
            targetConfig.Activate();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AvailableGatewayOption>> GetAvailableGatewaysAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<StorePaymentConfiguration> configurations =
            await GetAllConfigurationsAsync(storeId, paymentMethodId, cancellationToken);
        List<PaymentGateway> allGateways = await dbContext.Set<PaymentGateway>()
            .Where(pg => pg.IsActive)
            .ToListAsync(cancellationToken);

        List<AvailableGatewayOption> options = new();

        foreach (PaymentGateway gateway in allGateways)
        {
            StorePaymentConfiguration? config = configurations.FirstOrDefault(c => c.PaymentGatewayId == gateway.Id);

            options.Add(new AvailableGatewayOption(
                gateway.Id,
                gateway.Name,
                gateway.DisplayName,
                config?.IsActive ?? false,
                config is not null,
                null, // TODO: Add processing fee to configuration
                null // TODO: Add logo URL to gateway
            ));
        }

        return options;
    }
}