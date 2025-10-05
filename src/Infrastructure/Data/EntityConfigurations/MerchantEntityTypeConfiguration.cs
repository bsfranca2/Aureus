using Aureus.Domain.Merchants;
using Aureus.Infrastructure.Data.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.Data.EntityConfigurations;

public class MerchantEntityTypeConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> merchantConfiguration)
    {
        merchantConfiguration.ToTable("Merchants");

        merchantConfiguration.HasKey(m => m.Id);
        
        merchantConfiguration.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        merchantConfiguration.Property(m => m.Id)
            .HasConversion(new MerchantIdConverter());

        merchantConfiguration.Property(m => m.Name)
            .HasMaxLength(255)
            .IsRequired();

        merchantConfiguration.HasIndex(m => m.Name)
            .IsUnique();
    }
}
