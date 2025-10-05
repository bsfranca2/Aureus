using System;

using Ardalis.GuardClauses;

using Aureus.Domain.Shared.Exceptions;

namespace Aureus.Domain.Shared;

/// <summary>
///     Represents a monetary value with an ISO 4217 currency code.
/// </summary>
public readonly record struct Money
{
    private const int CurrencyLength = 3;

    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Guard.Against.NullOrWhiteSpace(currency);

        string normalizedCurrency = currency.Trim().ToUpperInvariant();
        Guard.Against.OutOfRange(normalizedCurrency.Length, nameof(currency), CurrencyLength, CurrencyLength);

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = normalizedCurrency;
    }

    public static Money Zero(string currency) => new(0m, currency);

    public Money Normalize() => new(decimal.Round(Amount, 2, MidpointRounding.AwayFromZero), Currency);

    public void EnsureSameCurrency(Money other)
    {
        if (!Currency.Equals(other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("Money currencies must match for this operation.");
        }
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }
}
