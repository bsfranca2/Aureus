using Ardalis.GuardClauses;

using Aureus.Domain.Merchants;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Shared.Exceptions;
using Aureus.Domain.Stores;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public sealed class Payment : IEntity<PaymentId>
{
    private readonly HashSet<PaymentAttempt> _attempts = [];

    public PaymentId Id { get; }
    public MerchantId MerchantId { get; }
    public StoreId StoreId { get; }
    public string OrderReference { get; }
    public Money Amount { get; }
    public PaymentStatus Status { get; private set; }
    public IdempotencyKey? IdempotencyKey { get; private set; }
    public IReadOnlyCollection<PaymentAttempt> Attempts => _attempts.ToList();
    public DateTime CreatedAt { get; }

    private Payment(PaymentId id, MerchantId merchantId, StoreId storeId, string orderReference, Money amount,
        PaymentStatus status, IdempotencyKey? idempotencyKey, DateTime createdAt)
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

    public static Payment Create(
        MerchantId merchantId,
        StoreId storeId,
        string orderReference,
        Money amount,
        IdempotencyKey? idempotencyKey = null)
    {
        Guard.Against.NegativeOrZero(amount.Amount); // TODO: Refactor

        return new Payment(new PaymentId(), merchantId, storeId, orderReference, amount.Normalize(),
            PaymentStatus.Created, idempotencyKey, DateTime.UtcNow);
    }

    public PaymentAttempt AddAttempt(
        string provider,
        PaymentMethod method,
        Money amount)
    {
        PaymentAttempt attempt = PaymentAttempt.Create(Id, provider, method, amount);
        _attempts.Add(attempt);
        Status = PaymentStatus.Processing;
        return attempt;
    }

    public void MarkSucceeded(PaymentAttempt attempt)
    {
        attempt.MarkSucceeded();
        Status = PaymentStatus.Succeeded;
    }

    public void MarkFailed(PaymentAttempt attempt, string reason)
    {
        attempt.MarkFailed(reason);
        if (_attempts.All(a => a.Status == AttemptStatus.Failed))
        {
            Status = PaymentStatus.Failed;
        }
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Succeeded)
        {
            throw new DomainException("Only succeeded payments can be refunded");
        }

        Status = PaymentStatus.Refunded;
    }
}