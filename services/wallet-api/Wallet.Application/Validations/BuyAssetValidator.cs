using FluentValidation;
using Wallet.Application.DTOs;
using Wallet.Domain.Entities.Assets;

namespace Wallet.Application.Validations;

public class BuyAssetValidator : AbstractValidator<BuyAssetRequest>
{
    public BuyAssetValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty().WithMessage("O Ticker é obrigatório.")
            .MaximumLength(10).WithMessage("O Ticker deve ter no máximo 10 caracteres.")
            // Evita ArgumentNullException se o JSON vier com ticker null (Must roda após NotEmpty no mesmo pipeline).
            .Must(t => t != null && t.All(char.IsLetterOrDigit)).WithMessage("Ticker deve ser alfanumérico.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

        RuleFor(x => x.Type)
            .IsEnumName(typeof(AssetType), caseSensitive: false)
            .WithMessage("Tipo de ativo inválido.");
    }
}