using Wallet.Application.DTOs;
using Wallet.Domain.Repositories;

namespace Wallet.Application.UseCases.GetAssets;

public class GetAssetsQuery(IAssetRepository repository)
{
    public async Task<IEnumerable<AssetResponse>> ExecuteAsync()
    {
        var assets = await repository.GetAllAsync();

        return assets.Select(a => new AssetResponse(
            a.Id,
            a.Ticker,
            a.Name,
            a.Type.ToString(),
            a.Quantity,
            a.AveragePrice.Amount,
            a.AveragePrice.Currency,
            a.CurrentPrice
        ));
    }
}