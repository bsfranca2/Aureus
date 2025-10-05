namespace Aureus.Domain.Payments;

public sealed record Payer(
    Guid? Id,
    string? Email,
    string? FullName = null,
    string? DocumentType = null,
    string? DocumentNumber = null
);