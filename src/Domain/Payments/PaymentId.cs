namespace Aureus.Domain.Payments;

public readonly record struct PaymentId(Guid Value)
{
    public PaymentId() : this(Guid.CreateVersion7()) { }
}