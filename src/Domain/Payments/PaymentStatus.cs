namespace Aureus.Domain.Payments;

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Succeeded = 2,
    Failed = 3,
    Expired = 4,
    Cancelled = 5,
    Refunded = 6,
    PartiallyPaid = 7
}