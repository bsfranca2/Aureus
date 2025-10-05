using Aureus.Domain.Configuration;
using Aureus.Domain.Gateways;
using Aureus.Domain.Merchants;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Stores;

using Microsoft.EntityFrameworkCore;

namespace Aureus.Infrastructure.Data.ValueGenerators;

public static class DatabaseContextSeed
{
    public static async Task Seed(DatabaseContext context, CancellationToken cancellationToken = default)
    {
        await SeedPayments(context, cancellationToken);
        await SeedStoresAndProducts(context, cancellationToken);
        await SeedStoreConfigurations(context, cancellationToken);
    }

    private static async Task SeedPayments(DatabaseContext context, CancellationToken cancellationToken = default)
    {
        if (await context.PaymentMethods.AnyAsync(cancellationToken))
        {
            return;
        }

        PaymentMethod creditCardMethod = PaymentMethod.Create("Credit Card", PaymentMethodType.CreditCard);
        await context.AddAsync(creditCardMethod, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        PaymentGateway mercadoPagoGateway =
            PaymentGateway.Create("mercadopago", "Mercado Pago", PaymentGatewayType.MercadoPago);
        await context.AddAsync(mercadoPagoGateway, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedStoresAndProducts(DatabaseContext context,
        CancellationToken cancellationToken = default)
    {
        if (await context.Merchants.AnyAsync(cancellationToken))
        {
            return;
        }

        Merchant merchant = Merchant.Create("João Ap");
        await context.AddAsync(merchant, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        Store clothesStore = Store.Create(merchant.Id, "Clothes Store");
        await context.AddAsync(clothesStore, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        Store shoesStore = Store.Create(merchant.Id, "Shoes Store");
        await context.AddAsync(shoesStore, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedStoreConfigurations(DatabaseContext context,
        CancellationToken cancellationToken = default)
    {
        if (await context.StorePaymentConfigurations.AnyAsync(cancellationToken))
        {
            return;
        }

        List<StoreId> stores = await context.Stores
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        PaymentMethod creditCardMethod = await context.PaymentMethods
            .FirstAsync(pt => pt.Type == PaymentMethodType.CreditCard, cancellationToken);

        PaymentGateway mercadoPagoGateway = await context.PaymentGateways
            .FirstAsync(pg => pg.Type == PaymentGatewayType.MercadoPago, cancellationToken);

        foreach (StoreId storeId in stores)
        {
            PaymentGatewayCredentials credentials = new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            StorePaymentConfiguration storePaymentConfiguration = StorePaymentConfiguration
                .Create(storeId, creditCardMethod.Id, mercadoPagoGateway.Id, credentials);

            await context.AddAsync(storePaymentConfiguration, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}