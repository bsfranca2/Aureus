using Aureus.Domain.Configuration;
using Aureus.Domain.Gateways;
using Aureus.Domain.Merchants;
using Aureus.Domain.Outbox;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;
using Aureus.Domain.Stores;
using Aureus.Infrastructure.Data.EntityConfigurations;

using Microsoft.EntityFrameworkCore;

namespace Aureus.Infrastructure.Data;

/// <remarks>
///     Add migrations using the following command inside the 'src\Infrastructure' project directory:
///     dotnet ef migrations add [migration-name]
/// </remarks>
public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Merchant> Merchants { get; init; }
    public DbSet<Store> Stores { get; init; }
    public DbSet<Payment> Payments { get; init; }
    public DbSet<PaymentMethod> PaymentMethods { get; init; }
    public DbSet<PaymentGateway> PaymentGateways { get; init; }
    public DbSet<StorePaymentConfiguration> StorePaymentConfigurations { get; init; }
    public DbSet<OutboxMessage> OutboxMessages { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MerchantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StoreEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentGatewayEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StorePaymentConfigurationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
    }
}