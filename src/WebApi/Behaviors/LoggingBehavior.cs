using System.Diagnostics;
using System.Reflection;

using MediatR;

namespace Aureus.WebApi.Behaviors;

/// <summary>
///     Adds logging for all requests in MediatR pipeline.
///     Configure by adding the service with a scoped lifetime
///     Example for Autofac:
///     builder
///     .RegisterType&lt;Mediator&gt;()
///     .As&lt;IMediator&gt;()
///     .InstancePerLifetimeScope();
///     builder
///     .RegisterGeneric(typeof(LoggingBehavior&lt;,&gt;))
///     .As(typeof(IPipelineBehavior&lt;,&gt;))
///     .InstancePerLifetimeScope();
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<Mediator> _logger;

    public LoggingBehavior(ILogger<Mediator> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

            Type myType = request.GetType();
            PropertyInfo[] props = myType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                bool isRedacted = prop.GetCustomAttribute<RedactAttribute>() != null;
                object? propValue = isRedacted ? "REDACTED" : prop.GetValue(request, null);

                _logger.LogInformation("Property {Property} : {@Value}", prop.Name, propValue);
            }
        }

        Stopwatch sw = Stopwatch.StartNew();

        TResponse response = await next(cancellationToken);

        _logger.LogInformation("Handled {RequestName} with {Response} in {ms} ms", typeof(TRequest).Name, response,
            sw.ElapsedMilliseconds);
        sw.Stop();
        return response;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RedactAttribute : Attribute
{
}