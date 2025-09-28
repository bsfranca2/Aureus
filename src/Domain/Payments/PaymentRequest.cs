using Aureus.Domain.Methods;

namespace Aureus.Domain.Payments;

public sealed class PaymentRequest
{
    public decimal Amount { get; }
    public PaymentMethodType PaymentMethodType { get; }
    public Customer? Customer { get; }
    public Dictionary<string, object> PaymentData { get; }
    public string? Description { get; }
    public string? ExternalReference { get; }

    public PaymentRequest(
        decimal amount,
        PaymentMethodType paymentMethodType,
        Dictionary<string, object> paymentData,
        Customer? customer = null,
        string? description = null,
        string? externalReference = null)
    {
        Amount = amount;
        PaymentMethodType = paymentMethodType;
        PaymentData = paymentData;
        Customer = customer;
        Description = description;
        ExternalReference = externalReference;
    }
}