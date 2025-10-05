namespace Aureus.Domain.Payments;

public enum AttemptStatus
{
    Pending = 0,
    Processing = 1,
    Succeeded = 2,
    Failed = 3,
    Cancelled = 4,
    Expired = 5,
    Reversed = 6,
    Retrying = 7
}
