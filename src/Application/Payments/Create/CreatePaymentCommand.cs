using Ardalis.Result;

using Aureus.Domain.PaymentMethods;

using Bsfranca2.Core;

namespace Aureus.Application.Payments.Create;

public sealed record CreatePaymentCommand(
    Guid StoreId,
    string OrderReference,
    decimal Amount,
    string Currency,
    PaymentMethodId PaymentMethodId,
    PaymentMethodType PaymentMethodType,
    Dictionary<string, object> PaymentData,
    string? Description,
    string? PayerEmail,
    string? PayerFullName,
    string? PayerDocumentType,
    string? PayerDocumentNumber,
    string? IdempotencyKey
) : ICommand<Result<CreatePaymentResultDto>>;