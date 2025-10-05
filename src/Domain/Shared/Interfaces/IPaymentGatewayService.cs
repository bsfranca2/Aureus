using Aureus.Domain.Gateways;
using Aureus.Domain.Payments;

namespace Aureus.Domain.Shared.Interfaces;

public interface IPaymentGatewayService
{
    PaymentGatewayType GatewayType { get; }
    Task<PaymentProcessingResult> ProcessPaymentAsync(PaymentRequest request, PaymentGatewayCredentials credentials, CancellationToken ct = default);
    Task<PaymentValidationResult> ValidatePaymentAsync(string gatewayTransactionId, PaymentGatewayCredentials credentials, CancellationToken ct = default);
    Task<RefundResult> RefundPaymentAsync(string gatewayTransactionId, decimal amount, PaymentGatewayCredentials credentials, CancellationToken ct = default);
}