using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Stores;

namespace Aureus.Domain.Configuration;

/// <summary>
///     Service for managing store payment configurations
/// </summary>
public interface IStorePaymentConfigurationService
{
    /// <summary>
    ///     Gets the active payment configuration for a store and payment method
    /// </summary>
    Task<StorePaymentConfiguration?> GetActiveConfigurationAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all payment configurations for a store and payment method (for admin/dashboard)
    /// </summary>
    Task<IEnumerable<StorePaymentConfiguration>> GetAllConfigurationsAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a specific payment configuration
    /// </summary>
    Task<StorePaymentConfiguration?> GetConfigurationAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId gatewayId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sets a gateway as active for a payment method (deactivates others)
    /// </summary>
    Task SetActiveGatewayAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId gatewayId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets available gateway options for frontend display
    /// </summary>
    Task<IEnumerable<AvailableGatewayOption>> GetAvailableGatewaysAsync(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken cancellationToken = default);
}