namespace Aureus.Domain.Shared.Interfaces;

public interface IIdempotencyRepository
{
    Task<Guid?> TryGetAsync(
        Guid storeId,
        string externalReference,
        string idempotencyKey,
        CancellationToken ct = default);
    
    Task SaveAsync(
        Guid storeId,
        string externalReference,
        string idempotencyKey,
        Guid paymentId,
        CancellationToken ct = default);
}