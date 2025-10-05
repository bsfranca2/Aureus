using System.Text;
using System.Text.Json;

using Ardalis.Result;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IArdalisResult = Ardalis.Result.IResult;

namespace Aureus.Presentation;

public static partial class ResultExtensions
{
    /// <summary>
    /// Convert a <see cref="Result{T}"/> to an instance of <see cref="Microsoft.AspNetCore.Http.IResult"/>
    /// </summary>
    /// <typeparam name="T">The value being returned</typeparam>
    /// <param name="result">The Ardalis.Result to convert to an Microsoft.AspNetCore.Http.IResult</param>
    /// <returns></returns>
    public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult<T>(this Result<T> result) => ToMinimalApiResult((IArdalisResult)result);

    /// <summary>
    /// Convert a <see cref="Result"/> to an instance of <see cref="Microsoft.AspNetCore.Http.IResult"/>
    /// </summary>
    /// <param name="result">The Ardalis.Result to convert to an Microsoft.AspNetCore.Http.IResult</param>
    /// <returns></returns>
    public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult(this Result result) => ToMinimalApiResult((IArdalisResult)result);

    internal static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult(this IArdalisResult result) =>
        result.Status switch
        {
            ResultStatus.Ok => result is Result ? Results.Ok() : Results.Ok(result.GetValue()),
            ResultStatus.Created => Results.Created("", result.GetValue()),
            ResultStatus.NoContent => Results.NoContent(),
            ResultStatus.NotFound => NotFoundEntity(result),
            ResultStatus.Unauthorized => UnAuthorized(result),
            ResultStatus.Forbidden => Forbidden(result),
            ResultStatus.Invalid => BadRequest(result),
            ResultStatus.Error => UnprocessableEntity(result),
            ResultStatus.Conflict => ConflictEntity(result),
            ResultStatus.Unavailable => UnavailableEntity(result),
            ResultStatus.CriticalError => CriticalEntity(result),
            _ => throw new NotSupportedException($"Result {result.Status} conversion is not supported."),
        };

    private static Microsoft.AspNetCore.Http.IResult BadRequest(IArdalisResult result)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var error in result.ValidationErrors)
        {
            var camelCaseIdentifier = JsonNamingPolicy.CamelCase.ConvertName(error.Identifier);

            if (errors.TryGetValue(camelCaseIdentifier, out string[]? value))
            {
                var existingErrors = value.ToList();
                existingErrors.Add(error.ErrorMessage);
                errors[camelCaseIdentifier] = existingErrors.ToArray();
            }
            else
            {
                errors[camelCaseIdentifier] = [error.ErrorMessage];
            }
        }

        return Results.BadRequest(new ProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Detail = "See the validationErrors property for details.",
            Extensions = new Dictionary<string, object?>
            {
                ["validationErrors"] = errors
            }
        });
    }


    private static Microsoft.AspNetCore.Http.IResult UnprocessableEntity(IArdalisResult result)
    {
        var details = new StringBuilder("");

        foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();

        return Results.UnprocessableEntity(new ProblemDetails
        {
            Title = "Something went wrong.",
            Detail = details.ToString(),
        });
    }

    private static Microsoft.AspNetCore.Http.IResult NotFoundEntity(IArdalisResult result)
    {
        var details = new StringBuilder("");

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();

            return Results.NotFound(new ProblemDetails
            {
                Title = "Resource not found.",
                Detail = details.ToString()
            });
        }
        else
        {
            return Results.NotFound();
        }
    }

    private static Microsoft.AspNetCore.Http.IResult ConflictEntity(IArdalisResult result)
    {
        var details = new StringBuilder("");

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();

            return Results.Conflict(new ProblemDetails
            {
                Title = "There was a conflict.",
                Detail = details.ToString()
            });
        }
        else
        {
            return Results.Conflict();
        }
    }

    private static Microsoft.AspNetCore.Http.IResult CriticalEntity(IArdalisResult result)
    {
        var details = new StringBuilder("");

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();

            return Results.Problem(new ProblemDetails()
            {
                Title = "Something went wrong.",
                Detail = details.ToString(),
                Status = StatusCodes.Status500InternalServerError
            });
        }
        else
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private static Microsoft.AspNetCore.Http.IResult UnavailableEntity(IArdalisResult result)
    {
        var details = new StringBuilder("");

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();
            
            return Results.Problem(new ProblemDetails
            {
                Title = "Service unavailable.",
                Detail = details.ToString(),
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
        else
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static Microsoft.AspNetCore.Http.IResult Forbidden(IArdalisResult result)
    {
        var details = new StringBuilder("");

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();

            return Results.Problem(new ProblemDetails
            {
                Title = "Forbidden.",
                Detail = details.ToString(),
                Status = StatusCodes.Status403Forbidden
            });
        }
        else
        {
            return Results.Forbid();
        }
    }

    private static Microsoft.AspNetCore.Http.IResult UnAuthorized(IArdalisResult result)
    {
        var details = new StringBuilder("");

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors) details.Append("* ").Append(error).AppendLine();

            return Results.Problem(new ProblemDetails
            {
                Title = "Unauthorized.",
                Detail = details.ToString(),
                Status = StatusCodes.Status401Unauthorized
            });
        }
        else
        {
            return Results.Unauthorized();
        }
    }
}