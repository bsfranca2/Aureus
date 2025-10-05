using Aureus.Domain.Merchants;
using Aureus.Domain.Stores;
using Aureus.Infrastructure.Data.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.Data.EntityConfigurations;

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> storeConfiguration)
    {
        storeConfiguration.ToTable("Stores");

        storeConfiguration.HasKey(s => s.Id);
        
        storeConfiguration.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");
        
        storeConfiguration.Property(s => s.Id).HasConversion(new StoreIdConverter());
        
        storeConfiguration.Property(s => s.MerchantId).HasConversion(new MerchantIdConverter());
        
        storeConfiguration.HasOne<Merchant>().WithMany().HasForeignKey(s => s.MerchantId);

        storeConfiguration.Property(s => s.Name).HasMaxLength(150);
    }
}