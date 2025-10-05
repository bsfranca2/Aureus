using Aureus.Domain.Gateways;
using Aureus.Domain.Shared.Interfaces;

namespace Aureus.Infrastructure.EntityFramework.Repositories;

public class PaymentGatewayRepository(DatabaseContext dbContext)
    : Repository<PaymentGateway, PaymentGatewayId, DatabaseContext>(dbContext), IPaymentGatewayRepository
{
}