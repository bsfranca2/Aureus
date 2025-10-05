using Aureus.Domain.Payments;
using Aureus.Domain.Stores;

namespace Aureus.Domain.Shared.Interfaces;

public interface IIdempotencyRepository
{
    Task<Guid?> TryGetAsync(
        StoreId storeId,
        string externalReference,
        string idempotencyKey,
        CancellationToken ct = default);
    
    Task SaveAsync(
        StoreId storeId,
        string externalReference,
        string idempotencyKey,
        PaymentId paymentId,
        CancellationToken ct = default);
}