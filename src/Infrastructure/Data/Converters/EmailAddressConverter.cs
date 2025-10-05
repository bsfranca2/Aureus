using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.Data.Converters;

public class EmailAddressConverter() : ValueConverter<EmailAddress, string>(
    email => email.Value,
    value => new EmailAddress(value)
);