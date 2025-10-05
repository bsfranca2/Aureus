using Aureus.Domain.Merchants;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.EntityFramework.Converters;

public class MerchantIdConverter() : ValueConverter<MerchantId, long>(
    merchantId => merchantId.Value,
    value => new MerchantId((int)value)
);