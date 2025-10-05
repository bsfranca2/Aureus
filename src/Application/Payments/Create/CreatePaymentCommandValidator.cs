// using FluentValidation;
//
// namespace Aureus.Application.Payments.Create;
//
// public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
// {
//     public CreatePaymentCommandValidator()
//     {
//         RuleFor(x => x.MerchantId).NotEmpty();
//         RuleFor(x => x.StoreId).NotEmpty();
//         RuleFor(x => x.OrderReference).NotEmpty().MaximumLength(128);
//         RuleFor(x => x.Amount).GreaterThan(0m);
//         RuleFor(x => x.Currency).NotEmpty().Length(3);
//         RuleFor(x => x.PaymentMethodId).NotNull();
//         RuleFor(x => x.PaymentData).NotNull();
//         When(x => !string.IsNullOrWhiteSpace(x.IdempotencyKey), () =>
//         {
//             RuleFor(x => x.IdempotencyKey!).MaximumLength(128);
//         });
//     }
// }