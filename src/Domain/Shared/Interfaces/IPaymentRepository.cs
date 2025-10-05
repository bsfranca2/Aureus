using Aureus.Domain.Payments;

using Bsfranca2.Core;

namespace Aureus.Domain.Shared.Interfaces;

public interface IPaymentRepository : IRepository<Payment, PaymentId>
{
    Task<Payment?> GetByExternalReferenceAsync(Guid externalReference, CancellationToken ct);
    Task UpdateAttemptMetadataAsync(
        PaymentAttemptId attemptId,
        string? providerReferenceId,
        string? gatewayResponseJson,
        CancellationToken ct = default);
}