using Aureus.Domain.PaymentMethods;

using Bsfranca2.Core;

namespace Aureus.Domain.Payments;

public sealed class PaymentRequest
{
    public decimal Amount { get; }
    public PaymentMethodType PaymentMethodType { get; }
    public Dictionary<string, object> PaymentData { get; }
    public Payer? Payer { get; }
    public string? Description { get; }
    public string ExternalReference { get; }
    public IdempotencyKey? IdempotencyKey { get; private set; }

    public PaymentRequest(decimal amount, PaymentMethodType paymentMethodType, Dictionary<string, object> paymentData,
        Payer? payer, string? description, string externalReference, IdempotencyKey? idempotencyKey)
    {
        Amount = amount;
        PaymentMethodType = paymentMethodType;
        PaymentData = paymentData;
        Payer = payer;
        Description = description;
        ExternalReference = externalReference;
        IdempotencyKey = idempotencyKey;
    }
}