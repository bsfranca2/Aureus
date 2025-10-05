using System.Text.Json;

using Bsfranca2.Core;

namespace Aureus.Domain.Outbox;

public class OutboxMessage
{
    public Guid Id { get; }
    public string Type { get; }
    public string Payload { get; }
    public DateTime OccurredOnUtc { get; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

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

    public static OutboxMessage Create(IEvent @event)
    {
        return new OutboxMessage(
            @event.EventId,
            @event.GetType().FullName!,
            JsonSerializer.Serialize(@event, @event.GetType()),
            @event.OccurredOnUtc,
            null,
            null);
    }
}