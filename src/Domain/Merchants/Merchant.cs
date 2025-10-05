namespace Aureus.Domain.Merchants;

public class Merchant
{
    public MerchantId Id { get; private set; }
    public string Name { get; private set; }

    private Merchant(MerchantId id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Merchant Create(string name)
    {
        return new Merchant(new MerchantId(), name);
    }
}