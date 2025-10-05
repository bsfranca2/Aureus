using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public class PaymentAttempt : IEntity<PaymentAttemptId>
{
    public PaymentAttemptId Id { get; }
    public PaymentId PaymentId { get; }
    public PaymentMethodId PaymentMethodId { get; }
    public PaymentGatewayId PaymentProviderId { get; }
    public decimal Amount { get; }

    public AttemptStatus Status { get; private set; }
    public string? ProviderTransactionId { get; private set; }
    public string? ProviderResponse { get; private set; }
    public string? FailureReason { get; private set; }

    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private PaymentAttempt(
        PaymentAttemptId id,
        PaymentId paymentId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId paymentProviderId,
        decimal amount,
        AttemptStatus status,
        string? providerTransactionId,
        string? providerResponse,
        string? failureReason,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        PaymentId = paymentId;
        PaymentMethodId = paymentMethodId;
        PaymentProviderId = paymentProviderId;
        Amount = amount;
        Status = status;
        ProviderTransactionId = providerTransactionId;
        ProviderResponse = providerResponse;
        FailureReason = failureReason;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static PaymentAttempt Create(
        PaymentId paymentId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId paymentProviderId,
        decimal amount)
    {
        return new PaymentAttempt(
            new PaymentAttemptId(),
            paymentId,
            paymentMethodId,
            paymentProviderId,
            amount,
            AttemptStatus.Pending,
            null,
            null,
            null,
            DateTime.UtcNow,
            null);
    }

    public void MarkProcessing()
    {
        if (Status is not AttemptStatus.Pending and not AttemptStatus.Retrying)
            throw new InvalidOperationException($"Cannot move from {Status} to Processing.");

        Status = AttemptStatus.Processing;
    }

    public void MarkSucceeded(string providerTransactionId, string? providerResponse = null)
    {
        if (Status != AttemptStatus.Processing)
            throw new InvalidOperationException($"Cannot mark {Status} as Succeeded.");

        Status = AttemptStatus.Succeeded;
        ProviderTransactionId = providerTransactionId;
        ProviderResponse = providerResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason, string? providerResponse = null)
    {
        if (Status is AttemptStatus.Succeeded or AttemptStatus.Cancelled or AttemptStatus.Expired)
            throw new InvalidOperationException($"Cannot mark {Status} as Failed.");

        Status = AttemptStatus.Failed;
        FailureReason = reason;
        ProviderResponse = providerResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkRetrying()
    {
        if (Status != AttemptStatus.Failed)
            throw new InvalidOperationException($"Only Failed attempts can transition to Retrying.");

        Status = AttemptStatus.Retrying;
        FailureReason = null;
        UpdatedAt = null;
    }

    public void MarkCancelled(string? reason = null)
    {
        if (Status is AttemptStatus.Succeeded or AttemptStatus.Cancelled or AttemptStatus.Reversed)
            throw new InvalidOperationException($"Cannot cancel attempt in state {Status}.");

        Status = AttemptStatus.Cancelled;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkExpired()
    {
        if (Status is AttemptStatus.Succeeded or AttemptStatus.Cancelled or AttemptStatus.Reversed)
            throw new InvalidOperationException($"Cannot expire attempt in state {Status}.");

        Status = AttemptStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReversed(string? reason = null)
    {
        if (Status != AttemptStatus.Succeeded)
            throw new InvalidOperationException($"Only successful attempts can be reversed.");

        Status = AttemptStatus.Reversed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}
