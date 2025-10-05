using Aureus.Domain.PaymentMethods;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public class PaymentAttempt : IEntity<PaymentAttemptId>
{
    public PaymentAttemptId Id { get; }
    public PaymentId PaymentId { get; }
    public string Provider { get; }
    public PaymentMethod Method { get; }
    public Money Amount { get; }
    public AttemptStatus Status { get; private set; }
    public string? ProviderReferenceId { get; private set; }
    public string? ProviderResponse { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; }

    private PaymentAttempt(PaymentAttemptId id, PaymentId paymentId, string provider, PaymentMethod method,
        Money amount, AttemptStatus status, string? providerReferenceId, string? failureReason, DateTime createdAt)
    {
        Id = id;
        PaymentId = paymentId;
        Provider = provider;
        Method = method;
        Amount = amount;
        Status = status;
        ProviderReferenceId = providerReferenceId;
        FailureReason = failureReason;
        CreatedAt = createdAt;
    }

    public static PaymentAttempt Create(PaymentId paymentId, string provider, PaymentMethod method, Money amount)
    {
        return new PaymentAttempt(new PaymentAttemptId(), paymentId, provider, method, amount.Normalize(),
            AttemptStatus.Processing, null, null, DateTime.UtcNow);
    }

    public void MarkSucceeded(string? providerRef = null)
    {
        Status = AttemptStatus.Succeeded;
        ProviderReferenceId = providerRef;
    }

    public void MarkFailed(string reason)
    {
        Status = AttemptStatus.Failed;
        FailureReason = reason;
    }
}