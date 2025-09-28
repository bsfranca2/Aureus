using Aureus.Core;
using Aureus.Domain.Methods;

namespace Aureus.Domain;

public interface IPaymentMethodRepository : IRepository<PaymentMethod, PaymentMethodId>
{
    Task<PaymentMethod?> GetActiveByIdAsync(PaymentMethodId paymentMethodId);
    Task<PaymentMethod?> GetActiveByPaymentMethodTypeAsync(PaymentMethodType paymentMethodType);
}