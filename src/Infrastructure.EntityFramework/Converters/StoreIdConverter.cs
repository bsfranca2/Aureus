using Aureus.Domain.Stores;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.EntityFramework.Converters;

public class StoreIdConverter() : ValueConverter<StoreId, long>(
    storeId => storeId.Value,
    value => new StoreId((int)value)
);