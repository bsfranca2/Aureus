using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Shared.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Aureus.Infrastructure.EntityFramework.Repositories;

public class PaymentMethodRepository(DatabaseContext dbContext)
    : Repository<PaymentMethod, PaymentMethodId, DatabaseContext>(dbContext), IPaymentMethodRepository
{
    public async Task<PaymentMethod?> GetActiveByIdAsync(PaymentMethodId paymentMethodId)
    {
        return await DbSet
            .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId && pm.IsActive);
    }

    public async Task<PaymentMethod?> GetActiveByPaymentMethodTypeAsync(PaymentMethodType paymentMethodType)
    {
        return await DbSet
            .FirstOrDefaultAsync(pm => pm.Type == paymentMethodType && pm.IsActive);
    }
}