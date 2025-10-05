using Bsfranca2.Core;

namespace Aureus.Domain.PaymentMethods;

public sealed class PaymentMethod : IEntity<PaymentMethodId>
{
    public PaymentMethodId Id { get; }
    public string Name { get; }
    public PaymentMethodType Type { get; }
    public bool IsActive { get; private set; }

    private PaymentMethod(PaymentMethodId id, string name, PaymentMethodType type, bool isActive)
    {
        Id = id;
        Name = name;
        Type = type;
        IsActive = isActive;
    }

    public static PaymentMethod Create(string name, PaymentMethodType type)
    {
        return new PaymentMethod(new PaymentMethodId(), name, type, true);
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