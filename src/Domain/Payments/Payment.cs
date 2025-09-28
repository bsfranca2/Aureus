using Aureus.Core;
using Aureus.Domain.Gateways;
using Aureus.Domain.Methods;

namespace Aureus.Domain.Payments;

/// <summary>
///     Represents a payment transaction
/// </summary>
public sealed class Payment : IEntity<PaymentId>
{
    public PaymentId Id { get; }
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    public PaymentMethodId PaymentMethodId { get; }
    public PaymentGatewayId PaymentGatewayId { get; }
    public decimal Amount { get; }
    public PaymentStatus Status { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayResponse { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ProcessedAt { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment(
        PaymentId id,
        OrderId orderId,
        CustomerId customerId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId paymentGatewayId,
        decimal amount,
        PaymentStatus status,
        string? gatewayTransactionId,
        string? gatewayResponse,
        DateTime createdAt,
        DateTime? processedAt,
        string? failureReason)
    {
        Id = id;
        OrderId = orderId;
        CustomerId = customerId;
        PaymentMethodId = paymentMethodId;
        PaymentGatewayId = paymentGatewayId;
        Amount = amount;
        Status = status;
        GatewayTransactionId = gatewayTransactionId;
        GatewayResponse = gatewayResponse;
        CreatedAt = createdAt;
        ProcessedAt = processedAt;
        FailureReason = failureReason;
    }

    public static Payment Create(
        OrderId orderId,
        CustomerId customerId,
        PaymentMethodId paymentMethodId,
        PaymentGatewayId paymentGatewayId,
        decimal amount)
    {
        return new Payment(
            new PaymentId(),
            orderId,
            customerId,
            paymentMethodId,
            paymentGatewayId,
            amount,
            PaymentStatus.Pending,
            null,
            null,
            DateTime.UtcNow,
            null,
            null);
    }

    public void MarkAsProcessing(string gatewayTransactionId)
    {
        Status = PaymentStatus.Processing;
        GatewayTransactionId = gatewayTransactionId;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(string gatewayResponse)
    {
        Status = PaymentStatus.Completed;
        GatewayResponse = gatewayResponse;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string failureReason, string? gatewayResponse = null)
    {
        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        GatewayResponse = gatewayResponse;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
    }
}