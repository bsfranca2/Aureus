using Ardalis.GuardClauses;

using Bsfranca2.Core;

namespace Aureus.Domain.PaymentMethods;

public sealed class PaymentMethod : IEntity<PaymentMethodId>
{
    private PaymentMethod()
    {
        Name = string.Empty;
    }

    private PaymentMethod(PaymentMethodId id, string name, PaymentMethodType type, bool isActive)
    {
        Id = id;
        Name = name;
        Type = type;
        IsActive = isActive;
    }

    public PaymentMethodId Id { get; private set; }
    public string Name { get; private set; }
    public PaymentMethodType Type { get; private set; }
    public bool IsActive { get; private set; }

    public static PaymentMethod Create(string name, PaymentMethodType type)
    {
        Guard.Against.NullOrWhiteSpace(name);

        return new PaymentMethod(new PaymentMethodId(), name.Trim(), type, true);
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
