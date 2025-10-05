// using Ardalis.Result;
//
// using Aureus.Domain.Shared.Exceptions;
//
// using Bsfranca2.Core;
//
// namespace Aureus.Application.Payments.Create;
//
// internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Result<CreatePaymentResultDto>>
// {
//     public Task<Result<CreatePaymentResultDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
//     {
//         // // 1) Idempotência (short-circuit)
//         // if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
//         // {
//         //     var existing = await _idempotency.TryGetAsync(request.MerchantId, request.StoreId, request.OrderReference, request.IdempotencyKey!, cancellationToken);
//         //     if (existing is not null)
//         //         return new CreatePaymentResultDto(existing.Value.ToString());
//         // }
//         
//         // 2) Verifica se há configuração ativa do método para a store
//         var activeCfg = await _storeConfigs.GetActiveConfigurationAsync(new(cmd.StoreId), cmd.PaymentMethodId, ct);
//         if (activeCfg is null || !activeCfg.IsEnabled)
//             throw new DomainException("No active payment gateway configured for this store/method.");
//         
//         await _uow.BeginTransactionAsync(ct);
//         
//         var payment = new Payment(
//             merchantId: cmd.MerchantId,
//             storeId: cmd.StoreId,
//             orderReference: cmd.OrderReference,
//             amount: new Money(cmd.Amount, cmd.Currency),
//             idempotencyKey: string.IsNullOrWhiteSpace(cmd.IdempotencyKey) ? null : new IdempotencyKey(cmd.IdempotencyKey!)
//         );
//
//         await _payments.CreateAsync(payment, ct);
//         
//         // 4) Persiste vínculo de idempotência (se houver)
//         if (!string.IsNullOrWhiteSpace(cmd.IdempotencyKey))
//             await _idempotency.SaveAsync(cmd.MerchantId, cmd.StoreId, cmd.OrderReference, cmd.IdempotencyKey!, payment.Id.Value, ct);
//
//         // 5) Enfileira evento na Outbox para o worker processar o gateway
//         var evt = new ProcessPaymentRequested(
//             PaymentId: payment.Id.Value.ToString(),
//             MerchantId: cmd.MerchantId,
//             StoreId: cmd.StoreId,
//             OrderReference: cmd.OrderReference,
//             Amount: cmd.Amount,
//             Currency: cmd.Currency,
//             PaymentMethodId: cmd.PaymentMethodId.Value,
//             PaymentMethodType: (int)cmd.PaymentMethodType,
//             PaymentData: cmd.PaymentData,
//             Description: cmd.Description,
//             PayerEmail: cmd.PayerEmail,
//             PayerFullName: cmd.PayerFullName,
//             PayerDocumentType: cmd.PayerDocumentType,
//             PayerDocumentNumber: cmd.PayerDocumentNumber,
//             IdempotencyKey: cmd.IdempotencyKey
//         );
//         
//         var payload = JsonSerializer.Serialize(evt);
//         await _outbox.EnqueueAsync(type: nameof(ProcessPaymentRequested), payload: payload, ct);
//
//         await _uow.CommitTransactionAsync(ct);
//
//         // 6) Retorna só o PaymentId para consulta posterior
//         return new CreatePaymentResultDto(payment.Id.Value.ToString());
//     }
// }