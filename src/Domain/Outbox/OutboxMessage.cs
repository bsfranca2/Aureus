using System;
using System.Text.Json;

using Ardalis.GuardClauses;

using Bsfranca2.Core;

namespace Aureus.Domain.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
        Type = string.Empty;
        Payload = string.Empty;
    }

    private OutboxMessage(
        Guid id,
        string type,
        string payload,
        DateTime occurredOnUtc,
        DateTime? processedOnUtc,
        string? error)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredOnUtc = occurredOnUtc;
        ProcessedOnUtc = processedOnUtc;
        Error = error;
    }

    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

    public static OutboxMessage Create(IEvent @event)
    {
        Guard.Against.Null(@event);

        return new OutboxMessage(
            @event.EventId,
            @event.GetType().FullName ?? @event.GetType().Name,
            JsonSerializer.Serialize(@event, @event.GetType()),
            @event.OccurredOnUtc,
            null,
            null);
    }

    public void MarkProcessed(DateTime processedOnUtc)
    {
        EnsureValidTimestamp(processedOnUtc);

        ProcessedOnUtc = processedOnUtc;
        Error = null;
    }

    public void MarkFailed(string error, DateTime processedOnUtc)
    {
        Guard.Against.NullOrWhiteSpace(error);
        EnsureValidTimestamp(processedOnUtc);

        Error = error.Trim();
        ProcessedOnUtc = processedOnUtc;
    }

    private static void EnsureValidTimestamp(DateTime timestamp)
    {
        if (timestamp == default)
        {
            throw new ArgumentException("Processed timestamp must be specified.", nameof(timestamp));
        }
    }
}
