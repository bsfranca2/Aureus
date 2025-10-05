using Aureus.Domain.Payments;
using Aureus.Infrastructure.Data.Converters;

using Bsfranca2.Core;

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
            .HasConversion(new PaymentIdConverter())
            .ValueGeneratedNever();

        paymentConfiguration.Property(p => p.StoreId)
            .HasConversion(new StoreIdConverter());

        paymentConfiguration.Property(p => p.ExternalReference)
            .HasMaxLength(100);

        paymentConfiguration.Property(p => p.Amount)
            .HasPrecision(12, 2);
        
        paymentConfiguration.Property(p => p.Status);
        
        paymentConfiguration.Property(p => p.IdempotencyKey)
            .HasConversion(new NullableIdempotencyKeyConverter())
            .HasMaxLength(128);

        paymentConfiguration.Property(p => p.CreatedAt);
        paymentConfiguration.Property(p => p.UpdatedAt);
        
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
                .ValueGeneratedNever();

            attemptsConfiguration.Property(attempt => attempt.PaymentId)
                .HasConversion(new PaymentIdConverter());

            attemptsConfiguration.Property(attempt => attempt.PaymentMethodId)
                .HasConversion(new PaymentMethodIdConverter());

            attemptsConfiguration.Property(attempt => attempt.PaymentProviderId)
                .HasConversion(new PaymentGatewayIdConverter());
            
            attemptsConfiguration.Property(attempt => attempt.Amount)
                .HasPrecision(12, 2);

            attemptsConfiguration.Property(attempt => attempt.Status);

            attemptsConfiguration.Property(attempt => attempt.ProviderTransactionId);
            
            attemptsConfiguration.Property(attempt => attempt.ProviderResponse)
                .HasColumnType("text")
                .IsRequired(false);

            attemptsConfiguration.Property(attempt => attempt.FailureReason)
                .HasColumnType("text")
                .IsRequired(false);

            attemptsConfiguration.Property(attempt => attempt.CreatedAt);
            attemptsConfiguration.Property(attempt => attempt.UpdatedAt);
            
            attemptsConfiguration.HasIndex(attempt => attempt.Status);
            attemptsConfiguration.HasIndex(attempt => attempt.CreatedAt);
        });

        paymentConfiguration.HasIndex(p => p.StoreId);
        paymentConfiguration.HasIndex(p => p.ExternalReference);
        paymentConfiguration.HasIndex(p => p.Status);
        paymentConfiguration.HasIndex(p => p.CreatedAt);
    }
}