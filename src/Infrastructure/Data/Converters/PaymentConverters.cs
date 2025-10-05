using Aureus.Domain.Configuration;
using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.Data.Converters;

public class PaymentMethodIdConverter() : ValueConverter<PaymentMethodId, long>(
    id => id.Value,
    value => new PaymentMethodId(value)
);

public class PaymentGatewayIdConverter() : ValueConverter<PaymentGatewayId, long>(
    id => id.Value,
    value => new PaymentGatewayId(value)
);

public class StorePaymentConfigurationIdConverter() : ValueConverter<StorePaymentConfigurationId, long>(
    id => id.Value,
    value => new StorePaymentConfigurationId(value)
);

public class PaymentIdConverter() : ValueConverter<PaymentId, Guid>(
    id => id.Value,
    value => new PaymentId(value)
);