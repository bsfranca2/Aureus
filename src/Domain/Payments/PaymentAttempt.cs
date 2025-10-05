using System;

using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Shared;
using Aureus.Domain.Shared.Exceptions;

using Ardalis.GuardClauses;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public sealed class PaymentAttempt : IEntity<PaymentAttemptId>
{
    private PaymentAttempt()
    {
        Provider = string.Empty;
        Method = null!;
        Amount = default;
    }

    private PaymentAttempt(
        PaymentAttemptId id,
        PaymentId paymentId,
        string provider,
        PaymentMethod method,
        Money amount,
        AttemptStatus status,
        string? providerReferenceId,
        string? providerResponse,
        string? failureReason,
        DateTime createdAt)
    {
        Id = id;
        PaymentId = paymentId;
        Provider = provider;
        Method = method;
        Amount = amount;
        Status = status;
        ProviderReferenceId = providerReferenceId;
        ProviderResponse = providerResponse;
        FailureReason = failureReason;
        CreatedAt = createdAt;
    }

    public PaymentAttemptId Id { get; private set; }
    public PaymentId PaymentId { get; private set; }
    public string Provider { get; private set; }
    public PaymentMethod Method { get; private set; }
    public Money Amount { get; private set; }
    public AttemptStatus Status { get; private set; }
    public string? ProviderReferenceId { get; private set; }
    public string? ProviderResponse { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PaymentAttempt Create(PaymentId paymentId, string provider, PaymentMethod method, Money amount)
    {
        Guard.Against.Default(paymentId);
        Guard.Against.NullOrWhiteSpace(provider);
        Guard.Against.Null(method);
        Guard.Against.False(method.IsActive, "paymentMethod", "Payment method must be active.");
        Guard.Against.NegativeOrZero(amount.Amount);

        return new PaymentAttempt(
            new PaymentAttemptId(),
            paymentId,
            provider.Trim(),
            method,
            amount.Normalize(),
            AttemptStatus.Processing,
            null,
            null,
            null,
            DateTime.UtcNow);
    }

    public void MarkSucceeded(string? providerRef = null, string? providerResponse = null)
    {
        if (Status == AttemptStatus.Succeeded)
        {
            throw new DomainException("Payment attempt is already marked as succeeded.");
        }

        if (Status == AttemptStatus.Failed)
        {
            throw new DomainException("Failed payment attempts cannot transition to succeeded.");
        }

        Status = AttemptStatus.Succeeded;
        ProviderReferenceId = providerRef;
        ProviderResponse = providerResponse;
        FailureReason = null;
    }

    public void MarkFailed(string reason, string? providerResponse = null)
    {
        Guard.Against.NullOrWhiteSpace(reason);

        if (Status == AttemptStatus.Succeeded)
        {
            throw new DomainException("Succeeded payment attempts cannot transition to failed.");
        }

        if (Status == AttemptStatus.Failed)
        {
            throw new DomainException("Payment attempt is already marked as failed.");
        }

        Status = AttemptStatus.Failed;
        FailureReason = reason.Trim();
        ProviderResponse = providerResponse;
    }
}
