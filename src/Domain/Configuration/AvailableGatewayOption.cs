using Aureus.Domain.Gateways;

namespace Aureus.Domain.Configuration;

public record AvailableGatewayOption(
    PaymentGatewayId GatewayId,
    string GatewayName,
    string DisplayName,
    bool IsActive,
    bool IsConfigured,
    decimal? ProcessingFee,
    string? LogoUrl
);