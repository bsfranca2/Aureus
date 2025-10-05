using Ardalis.GuardClauses;

namespace Aureus.Domain.Merchants;

public sealed class Merchant
{
    private Merchant()
    {
        Name = string.Empty;
    }

    private Merchant(MerchantId id, string name)
    {
        Id = id;
        Name = name;
    }

    public MerchantId Id { get; private set; }
    public string Name { get; private set; }

    public static Merchant Create(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);

        return new Merchant(new MerchantId(), name.Trim());
    }
}
