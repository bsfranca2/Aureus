using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;
using Aureus.Domain.Payments.Events;
using Aureus.Domain.Shared.Interfaces;
using Aureus.Domain.Stores;

using Bsfranca2.Core;

using Microsoft.Extensions.Logging;

namespace Aureus.Application.Payments.ProcessRequested;

public class ProcessPaymentRequestedEventHandler(
    ILogger<ProcessPaymentRequestedEventHandler> logger,
    IUnitOfWork unitOfWork,
    IPaymentMethodRepository paymentMethodRepository,
    IPaymentProcessingService paymentProcessingService
) : IEventHandler<ProcessPaymentRequestedEvent>
{
    public async Task HandleAsync(ProcessPaymentRequestedEvent @event, CancellationToken cancellationToken = new())
    {
        logger.LogInformation("Processing payment for External Reference {ExternalReference}",
            @event.ExternalReference);

        await unitOfWork.BeginTransactionAsync();

        PaymentMethod? paymentMethod = await paymentMethodRepository
            .GetActiveByIdAsync(new PaymentMethodId(@event.PaymentMethodId));

        if (paymentMethod == null)
        {
            return;
        }

        PaymentRequest request = new(
            @event.Amount,
            paymentMethod.Type,
            @event.PaymentData,
            externalReference: @event.ExternalReference,
            payer: new Payer(
                null,
                @event.PayerEmail,
                @event.PayerFullName,
                @event.PayerDocumentType,
                @event.PayerDocumentNumber
            ),
            description: @event.Description,
            idempotencyKey: string.IsNullOrWhiteSpace(@event.IdempotencyKey)
                ? null
                : new IdempotencyKey(@event.IdempotencyKey!)
        );

        try
        {
            await paymentProcessingService.ProcessPaymentAsync(
                request,
                new StoreId(@event.StoreId),
                new PaymentMethodId(@event.PaymentMethodId),
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment in gateway for PaymentId {PaymentId}", @event.PaymentId);
        }
        
        await unitOfWork.CommitTransactionAsync();
    }
}