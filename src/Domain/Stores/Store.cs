using Ardalis.GuardClauses;

using Aureus.Domain.Merchants;

namespace Aureus.Domain.Stores;

public sealed class Store
{
    private Store()
    {
        Name = string.Empty;
    }

    private Store(StoreId id, MerchantId merchantId, string name)
    {
        Id = id;
        MerchantId = merchantId;
        Name = name;
    }

    public StoreId Id { get; private set; }
    public MerchantId MerchantId { get; private set; }
    public string Name { get; private set; }

    public static Store Create(MerchantId merchantId, string name)
    {
        Guard.Against.Default(merchantId);
        Guard.Against.NullOrWhiteSpace(name);

        return new Store(new StoreId(), merchantId, name.Trim());
    }
}
