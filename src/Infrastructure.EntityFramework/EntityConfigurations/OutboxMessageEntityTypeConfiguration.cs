using Aureus.Domain.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aureus.Infrastructure.EntityFramework.EntityConfigurations;

public class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> outboxMessageConfiguration)
    {
        outboxMessageConfiguration.ToTable("OutboxMessages");

        outboxMessageConfiguration.HasKey(om => om.Id);

        outboxMessageConfiguration.Property(om => om.Id);

        outboxMessageConfiguration.Property(om => om.Type)
            .HasMaxLength(255)
            .IsRequired();

        outboxMessageConfiguration.Property(om => om.Payload)
            .HasColumnType("text")
            .IsRequired();

        outboxMessageConfiguration.Property(om => om.OccurredOnUtc)
            .IsRequired();

        outboxMessageConfiguration.Property(om => om.ProcessedOnUtc);

        outboxMessageConfiguration.Property(om => om.Error)
            .HasColumnType("text");
        
        outboxMessageConfiguration
            .HasIndex(om => new { om.OccurredOnUtc, om.ProcessedOnUtc })
            .HasDatabaseName("IX_OutboxMessages_Unprocessed")
            .HasFilter("\"ProcessedOnUtc\" IS NULL")
            .IncludeProperties(om => new { om.Id, om.Type, om.Payload });
    }
}