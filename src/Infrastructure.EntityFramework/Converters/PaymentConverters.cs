using Aureus.Domain.Configuration;
using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;

using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.EntityFramework.Converters;

public class PaymentMethodIdConverter() : ValueConverter<PaymentMethodId, int>(
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

public class PaymentAttemptIdConverter() : ValueConverter<PaymentAttemptId, Guid>(
    id => id.Value,
    value => new PaymentAttemptId(value)
);

public class NullableIdempotencyKeyConverter() : ValueConverter<IdempotencyKey?, string?>(
    key => key.HasValue ? key.Value.ToString() : null,
    value => value == null ? null : new IdempotencyKey(value)
);