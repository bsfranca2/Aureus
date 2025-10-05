using Bsfranca2.Core;

namespace Aureus.Domain.Gateways;

public sealed class PaymentGateway : IEntity<PaymentGatewayId>
{
    public PaymentGatewayId Id { get; }
    public string Name { get; }
    public string DisplayName { get; }
    public bool IsActive { get; private set; }
    public PaymentGatewayType Type { get; }

    private PaymentGateway(PaymentGatewayId id, string name, string displayName, PaymentGatewayType type, bool isActive)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Type = type;
        IsActive = isActive;
    }

    public static PaymentGateway Create(string name, string displayName, PaymentGatewayType type)
    {
        return new PaymentGateway(new PaymentGatewayId(), name, displayName, type, true);
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