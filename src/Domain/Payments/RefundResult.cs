namespace Aureus.Domain.Payments;

public sealed class RefundResult
{
    public bool IsSuccess { get; }
    public string? RefundId { get; }
    public string? ErrorMessage { get; }

    public RefundResult(bool isSuccess, string? refundId = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        RefundId = refundId;
        ErrorMessage = errorMessage;
    }
}