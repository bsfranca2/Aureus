// using System.Text.Json;
//
// using Aureus.Domain.Configuration;
// using Aureus.Domain.Gateways;
// using Aureus.Domain.PaymentMethods;
// using Aureus.Domain.Payments;
// using Aureus.Domain.Shared.Exceptions;
// using Aureus.Domain.Shared.Interfaces;
// using Aureus.Domain.Stores;
//
// using Bsfranca2.Core;
//
// namespace Aureus.Infrastructure.Services;
//
// public sealed class PaymentProcessingService : IPaymentProcessingService
// {
//     private readonly IStorePaymentConfigurationService _storeConfigs;
//     private readonly IEnumerable<IPaymentGatewayService> _gateways;
//     private readonly IPaymentRepository _payments;
//     private readonly IPaymentGatewayRepository _gatewayRepo;
//
//     private readonly Dictionary<PaymentGatewayType, IPaymentGatewayService> _gatewayServices;
//
//     public PaymentProcessingService(
//         IStorePaymentConfigurationService storeConfigs,
//         IEnumerable<IPaymentGatewayService> gateways,
//         IPaymentRepository payments,
//         IPaymentGatewayRepository gatewayRepo)
//     {
//         _storeConfigs = storeConfigs;
//         _gateways = gateways;
//         _payments = payments;
//         _gatewayRepo = gatewayRepo;
//         _gatewayServices = _gateways.ToDictionary(g => g.GatewayType);
//     }
//
//     public async Task<Payment> ProcessPaymentAsync(
//         PaymentRequest request,
//         StoreId storeId,
//         PaymentMethodId paymentMethodId,
//         CancellationToken ct = default)
//     {
//         // 1. Obter configuração ativa do método para a store
//         var config = await _storeConfigs.GetActiveConfigurationAsync(storeId, paymentMethodId, ct);
//         if (config is null)
//             throw new DomainException("No active payment gateway configured for this store/method.");
//
//         // 2. Obter gateway
//         var gateway = await _gatewayRepo.GetByIdAsync(config.PaymentGatewayId);
//         if (gateway is null)
//             throw new DomainException($"Gateway {config.PaymentGatewayId.Value} not found.");
//
//         // 3. Localizar serviço correspondente ao tipo do gateway
//         if (!_gatewayServices.TryGetValue(gateway.Type, out var gatewayService))
//             throw new DomainException($"No service implementation found for gateway type {gateway.Type}.");
//
//         // 4. Buscar pagamento existente
//         if (!Guid.TryParse(request.ExternalReference, out var orderGuid))
//             throw new DomainException("Invalid external reference format.");
//
//         var payment = await _payments.GetByExternalReferenceAsync(orderGuid, ct)
//                       ?? throw new DomainException("Payment not found for external reference.");
//
//         // 5. Criar nova tentativa (attempt)
//         var method = new PaymentMethod("Dynamic", request.PaymentMethodType);
//         var attempt = payment.AddAttempt(gateway.Name, method, new Money(request.Amount, "BRL"));
//
//         await _payments.UpdateAsync(payment, ct);
//
//         // 6. Chamar o gateway real
//         try
//         {
//             var result = await gatewayService.ProcessPaymentAsync(request, config.Credentials, ct);
//
//             if (result.IsSuccess)
//             {
//                 if (result.Status == PaymentStatus.Succeeded || result.Status == PaymentStatus.Completed)
//                     payment.MarkSucceeded(attempt);
//                 else
//                     payment.MarkFailed(attempt, "Unexpected status: " + result.Status);
//
//                 // Armazena resposta bruta se necessário
//                 var metadata = JsonSerializer.Serialize(result.GatewayResponse);
//                 await _payments.UpdateAttemptMetadataAsync(attempt.Id, result.TransactionId, metadata, ct);
//             }
//             else
//             {
//                 payment.MarkFailed(attempt, result.ErrorMessage ?? "Unknown failure.");
//                 await _payments.UpdateAsync(payment, ct);
//                 throw new DomainException(result.ErrorMessage ?? "Payment failed at gateway.");
//             }
//
//             await _payments.UpdateAsync(payment, ct);
//             return payment;
//         }
//         catch (Exception ex)
//         {
//             payment.MarkFailed(attempt, $"Gateway processing error: {ex.Message}");
//             await _payments.UpdateAsync(payment, ct);
//             throw;
//         }
//     }
//
//     public async Task<PaymentValidationResult> ValidatePaymentAsync(
//         PaymentId paymentId,
//         CancellationToken ct = default)
//     {
//         var payment = await _payments.GetByIdAsync(paymentId, ct);
//         if (payment is null)
//             return new PaymentValidationResult(false, PaymentStatus.Failed, null, "Payment not found.");
//
//         var cfg = await _storeConfigs.GetActiveConfigurationAsync(new StoreId(payment.StoreId), new PaymentMethodId(Guid.Empty), ct);
//         if (cfg is null)
//             return new PaymentValidationResult(false, PaymentStatus.Failed, null, "Payment configuration not found.");
//
//         var gateway = await _gatewayRepo.GetByIdAsync(cfg.PaymentGatewayId);
//         if (gateway is null || !_gatewayServices.TryGetValue(gateway.Type, out var gatewayService))
//             return new PaymentValidationResult(false, PaymentStatus.Failed, null, "Gateway service not available.");
//
//         var lastAttempt = payment.Attempts.LastOrDefault();
//         if (lastAttempt?.ProviderReferenceId is null)
//             return new PaymentValidationResult(false, PaymentStatus.Failed, null, "No transaction id for validation.");
//
//         return await gatewayService.ValidatePaymentAsync(lastAttempt.ProviderReferenceId, cfg.Credentials, ct);
//     }
//
//     public async Task<RefundResult> RefundPaymentAsync(
//         PaymentId paymentId,
//         decimal? amount = null,
//         CancellationToken ct = default)
//     {
//         var payment = await _payments.GetByIdAsync(paymentId, ct);
//         if (payment is null)
//             return new RefundResult(false, null, "Payment not found.");
//
//         if (payment.Status != PaymentStatus.Succeeded)
//             return new RefundResult(false, null, "Only succeeded payments can be refunded.");
//
//         var lastAttempt = payment.Attempts.LastOrDefault();
//         if (lastAttempt?.ProviderReferenceId is null)
//             return new RefundResult(false, null, "No gateway transaction reference.");
//
//         var cfg = await _storeConfigs.GetActiveConfigurationAsync(new StoreId(payment.StoreId), new PaymentMethodId(Guid.Empty), ct);
//         if (cfg is null)
//             return new RefundResult(false, null, "Payment configuration not found.");
//
//         var gateway = await _gatewayRepo.GetByIdAsync(cfg.PaymentGatewayId);
//         if (gateway is null || !_gatewayServices.TryGetValue(gateway.Type, out var gatewayService))
//             return new RefundResult(false, null, "Gateway service not available.");
//
//         var result = await gatewayService.RefundPaymentAsync(lastAttempt.ProviderReferenceId, amount ?? payment.Amount.Amount, cfg.Credentials, ct);
//
//         if (result.IsSuccess)
//         {
//             payment.MarkRefunded();
//             await _payments.UpdateAsync(payment, ct);
//         }
//
//         return result;
//     }
// }
