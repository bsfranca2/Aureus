using Aureus.Core;
using Aureus.Domain.Gateways;

namespace Aureus.Domain;

public interface IPaymentGatewayRepository : IRepository<PaymentGateway, PaymentGatewayId>
{
}