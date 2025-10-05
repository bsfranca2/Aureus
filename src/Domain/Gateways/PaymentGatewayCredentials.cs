using Bsfranca2.Core;

namespace Aureus.Domain.Gateways;

/// <summary>
///     Encrypted credentials for payment gateway integration
/// </summary>
public sealed class PaymentGatewayCredentials : ValueObject
{
    public string PublicKey { get; }
    public string PrivateKey { get; }
    public string? WebhookSecret { get; }
    public Dictionary<string, string> AdditionalSettings { get; }

    public PaymentGatewayCredentials(
        string publicKey,
        string privateKey,
        string? webhookSecret = null,
        Dictionary<string, string>? additionalSettings = null)
    {
        PublicKey = publicKey;
        PrivateKey = privateKey;
        WebhookSecret = webhookSecret;
        AdditionalSettings = additionalSettings ?? new Dictionary<string, string>();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PublicKey;
        yield return PrivateKey;
        yield return WebhookSecret ?? string.Empty;
    }
}