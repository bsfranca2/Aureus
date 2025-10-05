using Ardalis.GuardClauses;

using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Shared.Exceptions;
using Aureus.Domain.Stores;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public sealed class Payment : IEntity<PaymentId>
{
    private readonly HashSet<PaymentAttempt> _attempts = [];

    public PaymentId Id { get; }
    public StoreId StoreId { get; }
    public string ExternalReference { get; }
    public decimal Amount { get; }
    public PaymentStatus Status { get; private set; }
    public IdempotencyKey? IdempotencyKey { get; private set; }
    public IReadOnlyCollection<PaymentAttempt> Attempts => _attempts.ToList();

    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private Payment(
        PaymentId id,
        StoreId storeId,
        string externalReference,
        decimal amount,
        PaymentStatus status,
        IdempotencyKey? idempotencyKey,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        StoreId = storeId;
        ExternalReference = externalReference;
        Amount = amount;
        Status = status;
        IdempotencyKey = idempotencyKey;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Payment Create(
        StoreId storeId,
        string externalReference,
        decimal amount,
        IdempotencyKey? idempotencyKey = null)
    {
        Guard.Against.NegativeOrZero(amount);

        return new Payment(
            new PaymentId(),
            storeId,
            externalReference,
            amount,
            PaymentStatus.Pending,
            idempotencyKey,
            DateTime.UtcNow,
            null);
    }

    public PaymentAttempt AddAttempt(
        PaymentMethodId methodId,
        PaymentGatewayId gatewayId,
        Money amount)
    {
        if (Status is PaymentStatus.Cancelled or PaymentStatus.Expired or PaymentStatus.Refunded)
        {
            throw new DomainException($"Cannot add attempt when payment is {Status}");
        }

        PaymentAttempt attempt = PaymentAttempt.Create(Id, methodId, gatewayId, amount.Amount);
        _attempts.Add(attempt);
        Status = PaymentStatus.Processing;
        return attempt;
    }

    public void MarkSucceeded(PaymentAttempt attempt, string providerTransactionId)
    {
        if (Status is PaymentStatus.Cancelled or PaymentStatus.Expired)
        {
            throw new DomainException($"Cannot mark {Status} payment as succeeded");
        }

        attempt.MarkSucceeded(providerTransactionId);
        Status = PaymentStatus.Succeeded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(PaymentAttempt attempt, string reason)
    {
        if (!_attempts.Contains(attempt))
        {
            throw new DomainException("Attempt does not belong to this payment.");
        }

        attempt.MarkFailed(reason);

        if (_attempts.All(a => a.Status == AttemptStatus.Failed))
        {
            Status = PaymentStatus.Failed;
        }
    }

    public void MarkProcessing()
    {
        if (Status is PaymentStatus.Cancelled or PaymentStatus.Expired or PaymentStatus.Refunded)
        {
            throw new DomainException($"Cannot move {Status} payment to Processing");
        }

        Status = PaymentStatus.Processing;
    }

    public void MarkCancelled(string? reason = null)
    {
        if (Status is PaymentStatus.Succeeded or PaymentStatus.Refunded)
        {
            throw new DomainException("Cannot cancel a completed or refunded payment.");
        }

        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        foreach (PaymentAttempt attempt in _attempts.Where(a =>
                     a.Status is not AttemptStatus.Succeeded and not AttemptStatus.Cancelled))
        {
            attempt.MarkCancelled(reason);
        }
    }

    public void MarkExpired()
    {
        if (Status is PaymentStatus.Succeeded or PaymentStatus.Refunded)
        {
            throw new DomainException("Cannot expire a completed or refunded payment.");
        }

        Status = PaymentStatus.Expired;
        UpdatedAt = DateTime.UtcNow;

        foreach (PaymentAttempt attempt in _attempts.Where(a =>
                     a.Status is not AttemptStatus.Succeeded and not AttemptStatus.Cancelled))
        {
            attempt.MarkExpired();
        }
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Succeeded)
        {
            throw new DomainException("Only succeeded payments can be refunded.");
        }

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasActiveAttempt()
    {
        return _attempts.Any(a =>
            a.Status is AttemptStatus.Pending or AttemptStatus.Processing or AttemptStatus.Retrying);
    }
}