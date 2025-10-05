using Ardalis.Result;

using Aureus.Domain.Configuration;
using Aureus.Domain.Outbox;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;
using Aureus.Domain.Payments.Events;
using Aureus.Domain.Shared.Exceptions;
using Aureus.Domain.Shared.Interfaces;
using Aureus.Domain.Stores;

using Bsfranca2.Core;

namespace Aureus.Application.Payments.Create;

internal sealed class CreatePaymentCommandHandler(
    IUnitOfWork unitOfWork,
    IWorkContext workContext,
    // IIdempotencyRepository idempotency,
    IStorePaymentConfigurationService storeConfigs,
    IPaymentMethodRepository paymentMethodRepository,
    IPaymentRepository paymentRepository,
    IOutboxRepository outboxRepository
) : ICommandHandler<CreatePaymentCommand, Result<CreatePaymentResultDto>>
{
    public async Task<Result<CreatePaymentResultDto>> Handle(CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var storeId = workContext.GetStoreId();
        
        // if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        // {
        //     Guid? existing = await idempotency.TryGetAsync(storeId, request.ExternalReference,
        //         request.IdempotencyKey!, cancellationToken);
        //     if (existing is not null)
        //     {
        //         return new CreatePaymentResultDto(existing.Value.ToString());
        //     }
        // }

        PaymentMethod? paymentMethod =
            await paymentMethodRepository.GetActiveByPaymentMethodTypeAsync(request.PaymentMethodType);

        if (paymentMethod is null)
        {
            return Result.Error("Payment method not active");
        }

        StorePaymentConfiguration? activeCfg = await storeConfigs.GetActiveConfigurationAsync(
            storeId, paymentMethod.Id, cancellationToken);
        if (activeCfg is null || !activeCfg.IsEnabled)
        {
            throw new DomainException("No active payment gateway configured for this store/method.");
        }

        await unitOfWork.BeginTransactionAsync();

        Payment payment = Payment.Create(
            storeId,
            request.ExternalReference,
            request.Amount,
            string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? null
                : new IdempotencyKey(request.IdempotencyKey!)
        );

        await paymentRepository.CreateAsync(payment);

        // if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        // {
        //     await idempotency.SaveAsync(storeId, request.ExternalReference, request.IdempotencyKey!,
        //         payment.Id, cancellationToken);
        // }

        ProcessPaymentRequestedEvent evt = new(
            payment.Id.Value.ToString(),
            storeId.Value,
            request.ExternalReference,
            request.Amount,
            paymentMethod.Id.Value,
            request.PaymentData,
            request.Description,
            request.PayerEmail,
            request.PayerFullName,
            request.PayerDocumentType,
            request.PayerDocumentNumber,
            request.IdempotencyKey
        );

        OutboxMessage paymentRequestedMessage = OutboxMessage.Create(evt);
        await outboxRepository.AddAsync(paymentRequestedMessage);

        await unitOfWork.CommitTransactionAsync();

        return new CreatePaymentResultDto(payment.Id.Value.ToString());
    }
}