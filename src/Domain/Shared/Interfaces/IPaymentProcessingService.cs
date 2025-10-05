using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;
using Aureus.Domain.Stores;

namespace Aureus.Domain.Shared.Interfaces;

public interface IPaymentProcessingService
{
    /// <summary>
    /// Processa o pagamento chamando o gateway configurado da store.
    /// Cria tentativa (attempt), atualiza status e persiste o resultado.
    /// </summary>
    Task<Payment> ProcessPaymentAsync(
        PaymentRequest request,
        StoreId storeId,
        PaymentMethodId paymentMethodId,
        CancellationToken ct = default);

    /// <summary>
    /// Valida o status atual do pagamento diretamente no gateway.
    /// </summary>
    Task<PaymentValidationResult> ValidatePaymentAsync(
        PaymentId paymentId,
        CancellationToken ct = default);

    /// <summary>
    /// Solicita o reembolso de um pagamento no gateway e atualiza o estado do Payment.
    /// </summary>
    Task<RefundResult> RefundPaymentAsync(
        PaymentId paymentId,
        decimal? amount = null,
        CancellationToken ct = default);
}
