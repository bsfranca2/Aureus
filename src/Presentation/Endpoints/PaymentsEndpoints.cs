using Ardalis.Result;

using Aureus.Application.Payments.Create;

using Carter;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Aureus.Presentation.Endpoints;

public class PaymentsEndpoints() : CarterModule("/payments")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/", CreatePayment)
            .WithName(nameof(CreatePayment));
    }

    private static async Task<IResult> CreatePayment(CreatePaymentCommand command, ISender sender)
    {
        Result<CreatePaymentResultDto> result = await sender.Send(command);
        return result.ToMinimalApiResult();
    }
}