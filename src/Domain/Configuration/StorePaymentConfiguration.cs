using Ardalis.GuardClauses;

using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Stores;

using Bsfranca2.Core;

namespace Aureus.Domain.Configuration;

/// <summary>
///     Configuration of a payment method for a specific store using a specific gateway
/// </summary>
public sealed class StorePaymentConfiguration : IEntity<StorePaymentConfigurationId>
{
    private StorePaymentConfiguration()
    {
        Credentials = null!;
    }

    private StorePaymentConfiguration(
        StorePaymentConfigurationId id,
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId paymentGatewayId,
        PaymentGatewayCredentials credentials,
        bool isEnabled,
        bool isActive)
    {
        Id = id;
        StoreId = storeId;
        PaymentMethodId = paymentMethodId;
        PaymentGatewayId = paymentGatewayId;
        Credentials = credentials;
        IsEnabled = isEnabled;
        IsActive = isActive;
    }

    public StorePaymentConfigurationId Id { get; private set; }
    public StoreId StoreId { get; private set; }
    public PaymentMethodId PaymentMethodId { get; private set; }
    public PaymentGatewayId PaymentGatewayId { get; private set; }
    public bool IsEnabled { get; private set; }
    public bool IsActive { get; private set; }
    public PaymentGatewayCredentials Credentials { get; private set; }

    public static StorePaymentConfiguration Create(
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId paymentGatewayId,
        PaymentGatewayCredentials credentials)
    {
        Guard.Against.Default(storeId);
        Guard.Against.Default(paymentMethodId);
        Guard.Against.Default(paymentGatewayId);
        Guard.Against.Null(credentials);

        return new StorePaymentConfiguration(
            new StorePaymentConfigurationId(),
            storeId,
            paymentMethodId,
            paymentGatewayId,
            credentials,
            true,
            false);
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateCredentials(PaymentGatewayCredentials newCredentials)
    {
        Guard.Against.Null(newCredentials);

        Credentials = newCredentials;
    }
}
