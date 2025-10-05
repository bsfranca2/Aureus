using System.Collections.Generic;

using Ardalis.GuardClauses;

using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Shared;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public sealed class PaymentRequest
{
    public PaymentRequest(
        Money amount,
        PaymentMethodType paymentMethodType,
        IDictionary<string, object> paymentData,
        Payer? payer,
        string? description,
        string externalReference,
        IdempotencyKey? idempotencyKey)
    {
        Guard.Against.NegativeOrZero(amount.Amount);
        Guard.Against.Null(paymentData);
        Guard.Against.NullOrWhiteSpace(externalReference);

        Amount = amount.Normalize();
        PaymentMethodType = paymentMethodType;
        PaymentData = new Dictionary<string, object>(paymentData);
        Payer = payer;
        Description = description?.Trim();
        ExternalReference = externalReference.Trim();
        IdempotencyKey = idempotencyKey;
    }

    public Money Amount { get; }
    public PaymentMethodType PaymentMethodType { get; }
    public IReadOnlyDictionary<string, object> PaymentData { get; }
    public Payer? Payer { get; }
    public string? Description { get; }
    public string ExternalReference { get; }
    public IdempotencyKey? IdempotencyKey { get; private set; }

    public void WithIdempotencyKey(IdempotencyKey key)
    {
        Guard.Against.Null(key);

        IdempotencyKey = key;
    }
}
