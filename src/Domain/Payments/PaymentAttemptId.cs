namespace Aureus.Domain.Payments;

public readonly record struct PaymentAttemptId(Guid Value)
{
    public PaymentAttemptId() : this(Guid.CreateVersion7()) { }
}