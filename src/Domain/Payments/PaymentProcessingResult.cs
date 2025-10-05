using System.Collections.Generic;

using Ardalis.GuardClauses;

namespace Aureus.Domain.Payments;

/// <summary>
/// Result of payment processing operation
/// </summary>
public sealed class PaymentProcessingResult
{
    public bool IsSuccess { get; }
    public string? TransactionId { get; }
    public PaymentStatus Status { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyDictionary<string, object> GatewayResponse { get; }

    private PaymentProcessingResult(
        bool isSuccess,
        PaymentStatus status,
        string? transactionId = null,
        string? errorMessage = null,
        Dictionary<string, object>? gatewayResponse = null)
    {
        IsSuccess = isSuccess;
        Status = status;
        TransactionId = transactionId;
        ErrorMessage = errorMessage;
        GatewayResponse = gatewayResponse is null
            ? new Dictionary<string, object>()
            : new Dictionary<string, object>(gatewayResponse);
    }

    public static PaymentProcessingResult Success(
        string transactionId, 
        PaymentStatus status,
        Dictionary<string, object>? gatewayResponse = null)
    {
        Guard.Against.NullOrWhiteSpace(transactionId);

        return new PaymentProcessingResult(true, status, transactionId.Trim(), null, gatewayResponse);
    }

    public static PaymentProcessingResult Failure(
        string errorMessage,
        Dictionary<string, object>? gatewayResponse = null)
    {
        Guard.Against.NullOrWhiteSpace(errorMessage);

        return new PaymentProcessingResult(false, PaymentStatus.Failed, null, errorMessage.Trim(), gatewayResponse);
    }
}