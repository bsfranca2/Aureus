namespace Aureus.Domain.Payments;

public sealed class PaymentValidationResult
{
    public bool IsValid { get; }
    public PaymentStatus Status { get; }
    public decimal? Amount { get; }
    public string? ErrorMessage { get; }

    public PaymentValidationResult(bool isValid, PaymentStatus status, decimal? amount = null, string? errorMessage = null)
    {
        IsValid = isValid;
        Status = status;
        Amount = amount;
        ErrorMessage = errorMessage;
    }
}