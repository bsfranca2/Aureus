using Aureus.Core;
using Aureus.Domain.Payments;

namespace Aureus.Domain;

public interface IPaymentRepository : IRepository<Payment, PaymentId>
{
}