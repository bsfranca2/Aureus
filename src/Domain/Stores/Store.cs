using Aureus.Domain.Merchants;

namespace Aureus.Domain.Stores;

public class Store
{
    public StoreId Id { get; private set; }
    public MerchantId MerchantId { get; private set; }
    public string Name { get; private set; }

    private Store(StoreId id, MerchantId merchantId, string name)
    {
        Id = id;
        MerchantId = merchantId;
        Name = name;
    }

    public static Store Create(MerchantId merchantId, string name)
    {
        return new Store(new StoreId(), merchantId, name);
    }
}