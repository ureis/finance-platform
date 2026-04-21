using FluentValidation;
using Wallet.Application.DTOs;
using Wallet.Domain.Entities.Assets;
using Wallet.Domain.Repositories;
using Wallet.Domain.ValueObjects;

namespace Wallet.Application.UseCases.BuyAsset;

public class BuyAssetUseCase(
    IAssetRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<BuyAssetRequest> validator)
{
    public async Task ExecuteAsync(BuyAssetRequest request)
    {
        // Dispara ValidationException se falhar (o Middleware captura)
        await validator.ValidateAndThrowAsync(request);

        var normalizedTicker = request.Ticker.Trim().ToUpperInvariant();
        var normalizedName = request.Name.Trim();

        // 1. Verificar se o ativo já existe
        var asset = await repository.GetByTickerAsync(normalizedTicker);
        var price = Money.Of(request.Price);

        if (asset == null)
        {
            // 2. Se não existe, cria um novo
            var assetType = Enum.Parse<AssetType>(request.Type, ignoreCase: true);
            asset = new Asset(Guid.NewGuid(), normalizedTicker, normalizedName, assetType);

            asset.AddPurchase(request.Quantity, price);
            await repository.AddAsync(asset);
        }
        else
        {
            // Ativo já veio rastreado do GetByTickerAsync — não chamar Update (evita concorrência incorreta no EF).
            asset.AddPurchase(request.Quantity, price);
        }

        await repository.PrepareAssetForPersistenceAsync(asset);
        await unitOfWork.SaveChangesAsync();
    }
}