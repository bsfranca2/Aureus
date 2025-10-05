using Aureus.Domain.Payments;

using Bsfranca2.Core;

namespace Aureus.Domain.Shared.Interfaces;

public interface IPaymentRepository : IRepository<Payment, PaymentId>
{
}