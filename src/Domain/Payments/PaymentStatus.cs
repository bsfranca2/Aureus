namespace Aureus.Domain.Payments;

public enum PaymentStatus
{
    Created = 0,
    Processing = 1,
    Succeeded = 2,
    Failed = 3,
    Refunded = 4
}