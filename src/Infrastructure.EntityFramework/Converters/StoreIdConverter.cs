using Aureus.Domain.Stores;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.EntityFramework.Converters;

public class StoreIdConverter() : ValueConverter<StoreId, Guid>(
    storeId => storeId.Value,
    value => new StoreId(value)
);