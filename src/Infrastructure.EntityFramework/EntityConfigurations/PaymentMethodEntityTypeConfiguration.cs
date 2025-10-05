using Aureus.Domain.PaymentMethods;
using Aureus.Infrastructure.EntityFramework.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.EntityFramework.EntityConfigurations;

public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> paymentMethodConfiguration)
    {
        paymentMethodConfiguration.ToTable("PaymentMethods");

        paymentMethodConfiguration.HasKey(pm => pm.Id);
        
        paymentMethodConfiguration.Property(pm => pm.Id)
            .HasConversion(new PaymentMethodIdConverter())
            .ValueGeneratedOnAdd()
            .HasIdentityOptions(startValue: 1000, incrementBy: 1) // TODO: Remove
            .HasColumnType("bigint");

        paymentMethodConfiguration.Property(pm => pm.Name)
            .HasMaxLength(100)
            .IsRequired();

        paymentMethodConfiguration.Property(pm => pm.Type);

        paymentMethodConfiguration.Property(pm => pm.IsActive)
            .IsRequired();

        paymentMethodConfiguration.HasIndex(pm => pm.Type);
        paymentMethodConfiguration.HasIndex(pm => new { pm.Name, pm.Type }).IsUnique();
    }
}