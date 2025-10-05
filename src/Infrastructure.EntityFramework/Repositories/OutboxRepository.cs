using Aureus.Domain.Outbox;
using Aureus.Domain.Shared.Interfaces;

namespace Aureus.Infrastructure.EntityFramework.Repositories;

public class OutboxRepository(DatabaseContext dbContext) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message)
    {
        await dbContext.OutboxMessages.AddAsync(message);
        await dbContext.SaveChangesAsync();
    }
}