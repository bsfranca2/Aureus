using Ardalis.Result;

using Aureus.Domain.PaymentMethods;

using Bsfranca2.Core;

namespace Aureus.Application.Payments.Create;

public sealed record CreatePaymentCommand(
    int StoreId,
    string ExternalReference,
    decimal Amount,
    PaymentMethodType PaymentMethodType,
    Dictionary<string, object> PaymentData,
    string? Description,
    string? PayerEmail,
    string? PayerFullName,
    string? PayerDocumentType,
    string? PayerDocumentNumber,
    string? IdempotencyKey
) : ICommand<Result<CreatePaymentResultDto>>;