using System.Reflection;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace Aureus.WebApi.Behaviors;

public class FluentValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator> _validators;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            ValidationContext<TRequest> context = new(request);

            ValidationResult[] validationResults =
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            List<ValidationError> resultErrors = validationResults.SelectMany(r => r.AsErrors()).ToList();
            List<ValidationFailure> failures =
                validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

#nullable disable
            if (failures.Count != 0)
            {
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    Type resultType = typeof(TResponse).GetGenericArguments()[0];
                    MethodInfo invalidMethod = typeof(Result<>)
                        .MakeGenericType(resultType)
                        .GetMethod(nameof(Result<int>.Invalid), new[] { typeof(List<ValidationError>) });

                    if (invalidMethod != null)
                    {
                        return (TResponse)invalidMethod.Invoke(null, new object[] { resultErrors });
                    }
                }
                else if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.Invalid(resultErrors);
                }
                else
                {
                    throw new ValidationException(failures);
                }
            }
#nullable enable
        }

        return await next(cancellationToken);
    }
}