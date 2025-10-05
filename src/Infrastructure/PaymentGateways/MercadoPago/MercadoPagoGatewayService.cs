using System.Text.Json;

using Aureus.Domain.Gateways;
using Aureus.Domain.PaymentMethods;
using Aureus.Domain.Payments;
using Aureus.Domain.Shared.Interfaces;

using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Error;

using Microsoft.Extensions.Logging;

using Payment = MercadoPago.Resource.Payment.Payment;

namespace Aureus.Infrastructure.PaymentGateways.MercadoPago;

public class MercadoPagoGatewayService(
    ILogger<MercadoPagoGatewayService> logger
) : IPaymentGatewayService
{
    public PaymentGatewayType GatewayType => PaymentGatewayType.MercadoPago;

    public async Task<PaymentProcessingResult> ProcessPaymentAsync(
        PaymentRequest request,
        PaymentGatewayCredentials credentials,
        CancellationToken ct = default)
    {
        try
        {
            RequestOptions requestOptions = new() { AccessToken = credentials.PrivateKey };

            // Idempotência com ExternalReference, se houver
            requestOptions.CustomHeaders.Add("x-idempotency-key",
                request.ExternalReference ?? Guid.NewGuid().ToString());

            PaymentCreateRequest mpRequest = BuildMercadoPagoRequest(request);
            PaymentClient client = new();

            try
            {
                Payment? payment = await client.CreateAsync(mpRequest, requestOptions, ct);
                string paymentId = payment.Id?.ToString() ?? Guid.NewGuid().ToString();
                PaymentStatus status = MapMercadoPagoStatus(payment.Status);

                string? rawResponse = payment.ApiResponse.Content;
                Dictionary<string, object> responseData =
                    JsonSerializer.Deserialize<Dictionary<string, object>>(rawResponse) ?? [];

                return PaymentProcessingResult.Success(
                    paymentId,
                    status,
                    responseData
                );
            }
            catch (MercadoPagoApiException apiEx)
            {
                Dictionary<string, object> errorResponse =
                    JsonSerializer.Deserialize<Dictionary<string, object>>(apiEx.ApiResponse.Content) ?? [];
                return PaymentProcessingResult.Failure(apiEx.Message, errorResponse);
            }
            catch (MercadoPagoException mex)
            {
                return PaymentProcessingResult.Failure(mex.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment with MercadoPago");
            return PaymentProcessingResult.Failure($"Gateway error: {ex.Message}");
        }
    }

    public Task<PaymentValidationResult> ValidatePaymentAsync(
        string gatewayTransactionId,
        PaymentGatewayCredentials credentials,
        CancellationToken ct = default)
    {
        // Poderá consultar MercadoPago.Client.Payment.PaymentClient.GetAsync()
        // para sincronizar status e atualizar o Payment
        throw new NotImplementedException();
    }

    public Task<RefundResult> RefundPaymentAsync(
        string gatewayTransactionId,
        decimal amount,
        PaymentGatewayCredentials credentials,
        CancellationToken ct = default)
    {
        // Poderá usar MercadoPago.Client.Refund.RefundClient.CreateAsync()
        throw new NotImplementedException();
    }

    private static PaymentCreateRequest BuildMercadoPagoRequest(PaymentRequest request)
    {
        return request.PaymentMethodType switch
        {
            PaymentMethodType.CreditCard => BuildCreditCardRequest(request),
            // PaymentMethodType.PIX => BuildPixRequest(request),
            // PaymentMethodType.Boleto => BuildBoletoRequest(request),
            _ => throw new NotSupportedException(
                $"Payment method {request.PaymentMethodType} not supported by MercadoPago")
        };
    }

    private static PaymentCreateRequest BuildCreditCardRequest(PaymentRequest request)
    {
        CardData cardData = ExtractCardData(request.PaymentData);
        IdentificationData idData = ExtractIdentificationData(request.PaymentData);

        return new PaymentCreateRequest
        {
            TransactionAmount = request.Amount,
            Token = cardData.Token,
            Description = request.Description ?? "Payment",
            Installments = cardData.Installments,
            PaymentMethodId = cardData.PaymentMethodId,
            Payer = new PaymentPayerRequest
            {
                Email = request.Payer?.Email,
                Identification = new IdentificationRequest
                {
                    Type = idData.IdentificationType, Number = idData.IdentificationNumber
                }
            },
            ExternalReference = request.ExternalReference
        };
    }

    private static CardData ExtractCardData(Dictionary<string, object> paymentData)
    {
        return new CardData
        {
            Token = paymentData.GetValueOrDefault("token")?.ToString() ?? "",
            Installments =
                int.TryParse(paymentData.GetValueOrDefault("installments")?.ToString(), out int inst) ? inst : 1,
            PaymentMethodId = paymentData.GetValueOrDefault("paymentMethodId")?.ToString() ?? ""
        };
    }

    private static IdentificationData ExtractIdentificationData(Dictionary<string, object> paymentData)
    {
        return new IdentificationData
        {
            IdentificationType = paymentData.GetValueOrDefault("identificationType")?.ToString() ?? "",
            IdentificationNumber = paymentData.GetValueOrDefault("identificationNumber")?.ToString() ?? ""
        };
    }

    private static PaymentStatus MapMercadoPagoStatus(string? mpStatus)
    {
        return mpStatus?.ToLower() switch
        {
            "pending" => PaymentStatus.Pending,
            "in_process" => PaymentStatus.Processing,
            "approved" => PaymentStatus.Succeeded,
            "rejected" => PaymentStatus.Failed,
            "cancelled" => PaymentStatus.Cancelled,
            "refunded" => PaymentStatus.Refunded,
            "authorized" => PaymentStatus.Processing,
            "partially_refunded" => PaymentStatus.PartiallyPaid,
            _ => PaymentStatus.Failed
        };
    }

    private record CardData
    {
        public string Token { get; init; } = "";
        public int Installments { get; init; } = 1;
        public string PaymentMethodId { get; init; } = "";
    }

    private record IdentificationData
    {
        public string IdentificationType { get; init; } = "";
        public string IdentificationNumber { get; init; } = "";
    }
}