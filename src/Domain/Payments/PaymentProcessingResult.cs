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
    public Dictionary<string, object> GatewayResponse { get; }

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
        GatewayResponse = gatewayResponse ?? new Dictionary<string, object>();
    }

    public static PaymentProcessingResult Success(
        string transactionId, 
        PaymentStatus status,
        Dictionary<string, object>? gatewayResponse = null)
    {
        return new PaymentProcessingResult(true, status, transactionId, null, gatewayResponse);
    }

    public static PaymentProcessingResult Failure(
        string errorMessage,
        Dictionary<string, object>? gatewayResponse = null)
    {
        return new PaymentProcessingResult(false, PaymentStatus.Failed, null, errorMessage, gatewayResponse);
    }
}