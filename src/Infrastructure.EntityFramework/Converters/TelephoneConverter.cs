using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.EntityFramework.Converters;

public class TelephoneConverter() : ValueConverter<Telephone, string>(
    telephone => telephone.Value,
    value => new Telephone(value)
);