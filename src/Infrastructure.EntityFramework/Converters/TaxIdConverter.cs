using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aureus.Infrastructure.EntityFramework.Converters;

// public record TaxIdRepresentation(Type Discriminator, string? Value);
//
// public static class TaxIdRepresentationConversions
// {
//     public static TaxIdRepresentation ToRepresentation(this TaxId taxId)
//     {
//         return taxId.Map(
//             cpf => new TaxIdRepresentation(taxId.GetType(), cpf.Value),
//             cnpj => new TaxIdRepresentation(cnpj.GetType(), cnpj.Value));
//     }
//
//     public static TaxId ToTaxId(this TaxIdRepresentation representation)
//     {
//         object[] args = representation.Value is not null ? [representation.Value] : [];
//         return (TaxId)Activator.CreateInstance(representation.Discriminator, args)!;
//     }
// }
//
// public class TaxIdConverter() : ValueConverter<TaxId, TaxIdRepresentation>(
//     taxId => taxId.ToRepresentation(),
//     representation => representation.ToTaxId()
// );

public class CpfConverter() : ValueConverter<Cpf, string>(
    cpf => cpf.Value,
    value => new Cpf(value)
);