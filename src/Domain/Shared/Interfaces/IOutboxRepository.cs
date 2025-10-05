using Aureus.Domain.Outbox;

namespace Aureus.Domain.Shared.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message);
}