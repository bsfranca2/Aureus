using System.Text.Json;

using Aureus.Domain.Configuration;
using Aureus.Domain.Gateways;
using Aureus.Domain.Stores;
using Aureus.Infrastructure.Data.Converters;

using MercadoPago.Resource.PaymentMethod;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.Data.EntityConfigurations;

public class StorePaymentConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<StorePaymentConfiguration>
{
    public void Configure(EntityTypeBuilder<StorePaymentConfiguration> storePaymentConfiguration)
    {
        storePaymentConfiguration.ToTable("StorePaymentConfigurations");

        storePaymentConfiguration.HasKey(spc => spc.Id);

        storePaymentConfiguration.Property(spc => spc.Id)
            .HasConversion(new StorePaymentConfigurationIdConverter())
            .ValueGeneratedOnAdd()
            .HasIdentityOptions(1000, 1)
            .HasColumnType("bigint");

        storePaymentConfiguration.Property(spc => spc.StoreId)
            .HasConversion(new StoreIdConverter());

        storePaymentConfiguration.Property(spc => spc.PaymentMethodId)
            .HasConversion(new PaymentMethodIdConverter());

        storePaymentConfiguration.Property(spc => spc.PaymentGatewayId)
            .HasConversion(new PaymentGatewayIdConverter());

        storePaymentConfiguration.Property(spc => spc.IsEnabled);

        storePaymentConfiguration.Property(spc => spc.IsActive);

        storePaymentConfiguration.OwnsOne(spc => spc.Credentials, credentialsBuilder =>
        {
            credentialsBuilder.Property(c => c.PublicKey)
                .HasColumnName("PublicKey")
                .HasMaxLength(500)
                .IsRequired();

            credentialsBuilder.Property(c => c.PrivateKey)
                .HasColumnName("PrivateKey")
                .HasMaxLength(500)
                .IsRequired();

            credentialsBuilder.Property(c => c.WebhookSecret)
                .HasColumnName("WebhookSecret")
                .HasMaxLength(500);

            credentialsBuilder.Property(c => c.AdditionalSettings)
                .HasColumnName("AdditionalSettings")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ??
                         new Dictionary<string, string>())
                .HasColumnType("jsonb");
        });

        storePaymentConfiguration.HasOne<Store>()
            .WithMany()
            .HasForeignKey(spc => spc.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        storePaymentConfiguration.HasOne<PaymentMethod>()
            .WithMany()
            .HasForeignKey(spc => spc.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        storePaymentConfiguration.HasOne<PaymentGateway>()
            .WithMany()
            .HasForeignKey(spc => spc.PaymentGatewayId)
            .OnDelete(DeleteBehavior.Restrict);

        storePaymentConfiguration.HasIndex(spc => spc.StoreId);
        storePaymentConfiguration.HasIndex(spc => new { spc.StoreId, spc.PaymentMethodId });
        storePaymentConfiguration.HasIndex(spc => new { spc.StoreId, spc.PaymentMethodId, spc.PaymentGatewayId })
            .IsUnique();
    }
}