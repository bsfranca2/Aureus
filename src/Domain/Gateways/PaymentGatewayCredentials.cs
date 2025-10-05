using System;
using System.Collections.Generic;
using System.Linq;

using Ardalis.GuardClauses;

using Bsfranca2.Core;

namespace Aureus.Domain.Gateways;

/// <summary>
///     Encrypted credentials for payment gateway integration
/// </summary>
public sealed class PaymentGatewayCredentials : ValueObject
{
    private readonly Dictionary<string, string> _additionalSettings;

    public PaymentGatewayCredentials(
        string publicKey,
        string privateKey,
        string? webhookSecret = null,
        Dictionary<string, string>? additionalSettings = null)
    {
        Guard.Against.NullOrWhiteSpace(publicKey);
        Guard.Against.NullOrWhiteSpace(privateKey);

        PublicKey = publicKey.Trim();
        PrivateKey = privateKey.Trim();
        WebhookSecret = webhookSecret?.Trim();
        _additionalSettings = additionalSettings is null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(additionalSettings, StringComparer.OrdinalIgnoreCase);
    }

    public string PublicKey { get; }
    public string PrivateKey { get; }
    public string? WebhookSecret { get; }
    public IReadOnlyDictionary<string, string> AdditionalSettings => _additionalSettings;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PublicKey;
        yield return PrivateKey;
        yield return WebhookSecret ?? string.Empty;

        foreach ((string key, string value) in _additionalSettings.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            yield return key;
            yield return value;
        }
    }
}
