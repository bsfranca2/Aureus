using Aureus.Domain.Merchants;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;
using Aureus.Domain.Stores;
using Aureus.Infrastructure.Data.Converters;

using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.Data.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> paymentConfiguration)
    {
        paymentConfiguration.ToTable("Payments");

        paymentConfiguration.HasKey(p => p.Id);

        paymentConfiguration.Property(p => p.Id)
            .HasConversion(new PaymentIdConverter())
            .ValueGeneratedNever()
            .HasColumnType("uuid");

        paymentConfiguration.Property(p => p.MerchantId)
            .HasConversion(new MerchantIdConverter())
            .HasColumnType("bigint")
            .IsRequired();

        paymentConfiguration.Property(p => p.StoreId)
            .HasConversion(new StoreIdConverter())
            .HasColumnType("bigint")
            .IsRequired();

        paymentConfiguration.Property(p => p.OrderReference)
            .HasMaxLength(100)
            .IsRequired();

        paymentConfiguration.ComplexProperty(p => p.Amount, ConfigureMoney);

        paymentConfiguration.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        paymentConfiguration.Property(p => p.IdempotencyKey)
            .HasConversion(new NullableIdempotencyKeyConverter())
            .HasMaxLength(128);

        paymentConfiguration.Property(p => p.CreatedAt)
            .IsRequired();

        paymentConfiguration.HasOne<Merchant>()
            .WithMany()
            .HasForeignKey(p => p.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        paymentConfiguration.HasOne<Store>()
            .WithMany()
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        paymentConfiguration
            .HasIndex(p => new { p.MerchantId, p.StoreId, p.OrderReference })
            .IsUnique();

        paymentConfiguration.HasIndex(p => p.MerchantId);
        paymentConfiguration.HasIndex(p => p.StoreId);
        paymentConfiguration.HasIndex(p => p.Status);
        paymentConfiguration.HasIndex(p => p.CreatedAt);

        paymentConfiguration.Navigation(p => p.Attempts)
            .HasField("_attempts")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        paymentConfiguration.OwnsMany(p => p.Attempts, attemptsConfiguration =>
        {
            attemptsConfiguration.ToTable("PaymentAttempts");

            attemptsConfiguration.WithOwner()
                .HasForeignKey(attempt => attempt.PaymentId);

            attemptsConfiguration.HasKey(attempt => attempt.Id);

            attemptsConfiguration.Property(attempt => attempt.Id)
                .HasConversion(new PaymentAttemptIdConverter())
                .ValueGeneratedNever()
                .HasColumnType("uuid");

            attemptsConfiguration.Property(attempt => attempt.PaymentId)
                .HasConversion(new PaymentIdConverter())
                .HasColumnType("uuid")
                .IsRequired();

            attemptsConfiguration.Property(attempt => attempt.Provider)
                .HasMaxLength(100)
                .IsRequired();

            attemptsConfiguration.ComplexProperty(attempt => attempt.Amount, ConfigureMoney);

            attemptsConfiguration.Property(attempt => attempt.Status)
                .HasConversion<int>()
                .IsRequired();

            attemptsConfiguration.Property(attempt => attempt.ProviderReferenceId)
                .HasMaxLength(150);

            attemptsConfiguration.Property(attempt => attempt.ProviderResponse)
                .HasColumnType("text");

            attemptsConfiguration.Property(attempt => attempt.FailureReason)
                .HasColumnType("text");

            attemptsConfiguration.Property(attempt => attempt.CreatedAt)
                .IsRequired();

            attemptsConfiguration.Property<long>("PaymentMethodId")
                .HasColumnName("PaymentMethodId")
                .HasColumnType("bigint")
                .IsRequired();

            attemptsConfiguration.HasOne(attempt => attempt.Method)
                .WithMany()
                .HasForeignKey("PaymentMethodId")
                .OnDelete(DeleteBehavior.Restrict);

            attemptsConfiguration.HasIndex("PaymentMethodId");
            attemptsConfiguration.HasIndex(attempt => attempt.Status);
            attemptsConfiguration.HasIndex(attempt => attempt.CreatedAt);
        });
    }

    private static void ConfigureMoney(ComplexPropertyBuilder<Money> moneyConfiguration)
    {
        moneyConfiguration.Property(money => money.Amount)
            .HasColumnName("Amount")
            .HasColumnType("numeric(18, 2)")
            .IsRequired();

        moneyConfiguration.Property(money => money.Currency)
            .HasColumnName("Currency")
            .HasColumnType("char(3)")
            .HasMaxLength(3)
            .IsRequired();
    }
}
