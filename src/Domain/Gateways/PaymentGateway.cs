using Ardalis.GuardClauses;

using Bsfranca2.Core;

namespace Aureus.Domain.Gateways;

public sealed class PaymentGateway : IEntity<PaymentGatewayId>
{
    private PaymentGateway()
    {
        Name = string.Empty;
        DisplayName = string.Empty;
    }

    private PaymentGateway(
        PaymentGatewayId id,
        string name,
        string displayName,
        PaymentGatewayType type,
        bool isActive)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Type = type;
        IsActive = isActive;
    }

    public PaymentGatewayId Id { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public bool IsActive { get; private set; }
    public PaymentGatewayType Type { get; private set; }

    public static PaymentGateway Create(string name, string displayName, PaymentGatewayType type)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(displayName);

        return new PaymentGateway(new PaymentGatewayId(), name.Trim(), displayName.Trim(), type, true);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
