using System.Collections.Concurrent;
using System.Text.Json;

using Aureus.Domain;

using Bsfranca2.Core;

using Dapper;

using Npgsql;

namespace Aureus.OutboxProcessing;

internal sealed class OutboxProcessor(NpgsqlDataSource dataSource, IEventPublisher eventPublisher)
{
    private const int BatchSize = 1000;
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    public async Task<int> Execute(CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        List<OutboxMessageQuery> messages = (await connection.QueryAsync<OutboxMessageQuery>(
            """
            SELECT "Id", "Type", "Payload"
            FROM "OutboxMessages"
            WHERE "ProcessedOnUtc" IS NULL
            ORDER BY "OccurredOnUtc" LIMIT @BatchSize
            FOR UPDATE SKIP LOCKED
            """,
            new { BatchSize },
            transaction)).AsList();

        ConcurrentQueue<OutboxUpdate> updateQueue = new();

        List<Task> publishTasks = messages
            .Select(message => PublishMessage(message, updateQueue, eventPublisher, cancellationToken))
            .ToList();

        await Task.WhenAll(publishTasks);

        foreach (OutboxUpdate message in updateQueue)
        {
            await connection.ExecuteAsync(
                """
                UPDATE "OutboxMessages"
                SET "ProcessedOnUtc" = @ProcessedOnUtc, "Error" = @Error
                WHERE "Id" = @Id
                """,
                message,
                transaction);
        }

        if (!updateQueue.IsEmpty)
        {
            string updateSql =
                """
                UPDATE "OutboxMessages"
                SET "ProcessedOnUtc" = v."ProcessedOnUtc",
                    "Error" = v."Error"
                FROM (VALUES
                    {0}
                ) AS v("Id", "ProcessedOnUtc", "Error")
                WHERE "OutboxMessages"."Id" = v."Id"::uuid
                """;

            List<OutboxUpdate> updates = updateQueue.ToList();
            string valuesList = string.Join(",",
                updateQueue.Select((_, i) => $"(@Id{i}, @ProcessedOnUtc{i}, @Error{i})"));

            DynamicParameters parameters = new();
            for (int i = 0; i < updateQueue.Count; i++)
            {
                parameters.Add($"@Id{i}", updates[i].Id.ToString());
                parameters.Add($"@ProcessedOnUtc{i}", updates[i].ProcessedOnUtc);
                parameters.Add($"@Error{i}", updates[i].Error);
            }

            string formattedSql = string.Format(updateSql, valuesList);

            await connection.ExecuteAsync(formattedSql, parameters, transaction);
        }

        await transaction.CommitAsync(cancellationToken);

        return messages.Count;
    }

    private static async Task PublishMessage(
        OutboxMessageQuery message,
        ConcurrentQueue<OutboxUpdate> updateQueue,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        try
        {
            Type messageType = GetOrAddMessageType(message.Type);
            object deserializedMessage = JsonSerializer.Deserialize(message.Payload, messageType)!;

            await eventPublisher.PublishAsync(deserializedMessage, messageType, cancellationToken);

            updateQueue.Enqueue(new OutboxUpdate(message.Id, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            updateQueue.Enqueue(new OutboxUpdate(message.Id, DateTime.UtcNow, ex.ToString()));
        }
    }

    private static Type GetOrAddMessageType(string typename)
    {
        return TypeCache.GetOrAdd(typename, name => DomainAssemblyReference.Assembly.GetType(name)!);
    }

    private readonly struct OutboxUpdate(Guid id, DateTime processedOnUtc, string? error = null)
    {
        public Guid Id { get; } = id;
        public DateTime ProcessedOnUtc { get; } = processedOnUtc;
        public string? Error { get; } = error;
    }

    private readonly record struct OutboxMessageQuery(
        Guid Id,
        string Type,
        string Payload
    );
}