using Bsfranca2.Core;

namespace Aureus.Domain.Payments.Events;

public sealed record ProcessPaymentRequestedEvent(
    string PaymentId,
    Guid StoreId,
    string ExternalReference,
    decimal Amount,
    int PaymentMethodId,
    Dictionary<string, object> PaymentData,
    string? Description,
    string? PayerEmail,
    string? PayerFullName,
    string? PayerDocumentType,
    string? PayerDocumentNumber,
    string? IdempotencyKey
) : BaseEvent;