using Aureus.Domain.Payments;
using Aureus.Domain.Shared.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Aureus.Infrastructure.EntityFramework.Repositories;

public class PaymentRepository(DatabaseContext context)
    : Repository<Payment, PaymentId, DatabaseContext>(context), IPaymentRepository
{
    public async Task<Payment?> GetByExternalReferenceAsync(Guid externalReference, CancellationToken ct)
    {
        // Considerando que ExternalReference é armazenado como string no domínio,
        // aqui fazemos comparação com ToString() para evitar problemas de tipo.
        return await DbSet
            .Include(p => p.Attempts)
            .FirstOrDefaultAsync(p => p.ExternalReference == externalReference.ToString(), ct);
    }

    public async Task UpdateAttemptMetadataAsync(
        PaymentAttemptId attemptId,
        string? providerReferenceId,
        string? gatewayResponseJson,
        CancellationToken ct = default)
    {
        var attempt = await DbContext.Set<PaymentAttempt>()
            .FirstOrDefaultAsync(a => a.Id == attemptId, ct);

        if (attempt is null)
            return;

        attempt.UpdateProviderMetadata(providerReferenceId, gatewayResponseJson);

        DbContext.Update(attempt);
        await DbContext.SaveChangesAsync(ct);
    }
}