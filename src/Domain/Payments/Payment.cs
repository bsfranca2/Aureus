using System;
using System.Collections.Generic;
using System.Linq;

using Aureus.Domain.Merchants;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Shared.Exceptions;
using Aureus.Domain.Stores;
using Aureus.Domain.Shared;

using Ardalis.GuardClauses;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public sealed class Payment : IEntity<PaymentId>
{
    private readonly HashSet<PaymentAttempt> _attempts = [];

    private Payment()
    {
        OrderReference = string.Empty;
        Amount = default;
    }

    private Payment(
        PaymentId id,
        MerchantId merchantId,
        StoreId storeId,
        string orderReference,
        Money amount,
        PaymentStatus status,
        IdempotencyKey? idempotencyKey,
        DateTime createdAt)
    {
        Id = id;
        MerchantId = merchantId;
        StoreId = storeId;
        OrderReference = orderReference;
        Amount = amount;
        Status = status;
        IdempotencyKey = idempotencyKey;
        CreatedAt = createdAt;
    }

    public PaymentId Id { get; private set; }
    public MerchantId MerchantId { get; private set; }
    public StoreId StoreId { get; private set; }
    public string OrderReference { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public IdempotencyKey? IdempotencyKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<PaymentAttempt> Attempts => _attempts;

    public static Payment Create(
        MerchantId merchantId,
        StoreId storeId,
        string orderReference,
        Money amount,
        IdempotencyKey? idempotencyKey = null)
    {
        Guard.Against.Default(merchantId);
        Guard.Against.Default(storeId);
        Guard.Against.NullOrWhiteSpace(orderReference);
        Guard.Against.NegativeOrZero(amount.Amount);

        return new Payment(
            new PaymentId(),
            merchantId,
            storeId,
            orderReference.Trim(),
            amount.Normalize(),
            PaymentStatus.Created,
            idempotencyKey,
            DateTime.UtcNow);
    }

    public PaymentAttempt AddAttempt(string provider, PaymentMethod method, Money amount)
    {
        Guard.Against.NullOrWhiteSpace(provider);
        Guard.Against.Null(method);
        Guard.Against.NegativeOrZero(amount.Amount);

        if (!method.IsActive)
        {
            throw new DomainException("Only active payment methods can be used to create attempts.");
        }

        if (Status is PaymentStatus.Succeeded or PaymentStatus.Refunded)
        {
            throw new DomainException("Cannot add attempts once the payment has completed processing.");
        }

        amount.EnsureSameCurrency(Amount);

        PaymentAttempt attempt = PaymentAttempt.Create(Id, provider.Trim(), method, amount);
        _attempts.Add(attempt);
        Status = PaymentStatus.Processing;
        return attempt;
    }

    public void MarkSucceeded(PaymentAttempt attempt, string? providerReference = null, string? providerResponse = null)
    {
        if (Status == PaymentStatus.Refunded)
        {
            throw new DomainException("Refunded payments cannot transition to succeeded.");
        }

        if (Status == PaymentStatus.Succeeded)
        {
            return;
        }

        EnsureAttemptBelongsToPayment(attempt);
        attempt.MarkSucceeded(providerReference, providerResponse);
        Status = PaymentStatus.Succeeded;
    }

    public void MarkFailed(PaymentAttempt attempt, string reason, string? providerResponse = null)
    {
        if (Status == PaymentStatus.Refunded)
        {
            throw new DomainException("Refunded payments cannot transition to failed.");
        }

        Guard.Against.NullOrWhiteSpace(reason);
        EnsureAttemptBelongsToPayment(attempt);

        attempt.MarkFailed(reason, providerResponse);
        if (_attempts.All(a => a.Status == AttemptStatus.Failed))
        {
            Status = PaymentStatus.Failed;
        }
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Succeeded)
        {
            throw new DomainException("Only succeeded payments can be refunded.");
        }

        Status = PaymentStatus.Refunded;
    }

    public void UpdateIdempotencyKey(IdempotencyKey idempotencyKey)
    {
        Guard.Against.Null(idempotencyKey);

        IdempotencyKey = idempotencyKey;
    }

    private void EnsureAttemptBelongsToPayment(PaymentAttempt attempt)
    {
        if (attempt.PaymentId != Id)
        {
            throw new DomainException("Attempt does not belong to this payment.");
        }

        if (_attempts.All(existing => existing.Id != attempt.Id))
        {
            throw new DomainException("Attempt does not belong to this payment.");
        }
    }
}