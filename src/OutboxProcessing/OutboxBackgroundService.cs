namespace Aureus.OutboxProcessing;

internal sealed class OutboxBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxBackgroundService> logger
) : BackgroundService
{
    private const int OutboxProcessorFrequency = 3;
    private readonly int _maxParallelism = 2;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting OutboxBackgroundService...");

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = _maxParallelism,
            CancellationToken = stoppingToken
        };
        try
        {
            await Parallel.ForEachAsync(
                Enumerable.Range(0, _maxParallelism),
                parallelOptions,
                async (_, token) => await ProcessOutboxMessages(token));
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("OutboxBackgroundService cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured in OutboxBackgroundService");
        }
        finally
        {
            logger.LogInformation("OutboxBackgroundService finished...");
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        OutboxProcessor outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

        while (!cancellationToken.IsCancellationRequested)
        {
            await outboxProcessor.Execute(cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequency), cancellationToken);
        }
    }
}