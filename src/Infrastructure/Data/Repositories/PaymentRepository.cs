using Aureus.Domain.Payments;
using Aureus.Domain.Shared.Interfaces;

namespace Aureus.Infrastructure.Data.Repositories;

public class PaymentRepository(DatabaseContext context)
    : Repository<Payment, PaymentId, DatabaseContext>(context), IPaymentRepository
{
}