using Aureus.Domain.Gateways;

using Bsfranca2.Core;

namespace Aureus.Domain.Shared.Interfaces;

public interface IPaymentGatewayRepository : IRepository<PaymentGateway, PaymentGatewayId>
{
}