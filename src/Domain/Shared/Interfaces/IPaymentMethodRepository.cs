using Aureus.Domain.PaymentMethods;

using Bsfranca2.Core;

namespace Aureus.Domain.Shared.Interfaces;

public interface IPaymentMethodRepository : IRepository<PaymentMethod, PaymentMethodId>
{
    Task<PaymentMethod?> GetActiveByIdAsync(PaymentMethodId paymentMethodId);
    Task<PaymentMethod?> GetActiveByPaymentMethodTypeAsync(PaymentMethodType paymentMethodType);
}