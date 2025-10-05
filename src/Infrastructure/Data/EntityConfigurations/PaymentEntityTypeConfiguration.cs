using Aureus.Domain.Payments;
using Aureus.Infrastructure.Data.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.Data.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> paymentConfiguration)
    {
        paymentConfiguration.ToTable("Payments");

        paymentConfiguration.HasKey(p => p.Id);
        
        paymentConfiguration.Property(p => p.Id)
            .HasConversion(new PaymentIdConverter());

        paymentConfiguration.Property(p => p.Amount)
            .HasPrecision(12, 2)
            .IsRequired();

        paymentConfiguration.Property(p => p.Status);

        paymentConfiguration.Property(p => p.CreatedAt)
            .IsRequired();
        
        paymentConfiguration.HasIndex(p => p.Status);
        paymentConfiguration.HasIndex(p => p.CreatedAt);
    }
}