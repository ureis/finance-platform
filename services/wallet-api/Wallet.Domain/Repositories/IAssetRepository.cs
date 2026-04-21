namespace Wallet.Domain.Repositories;

using Wallet.Domain.Entities.Assets;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id);
    Task<IEnumerable<Asset>> GetAllAsync();
    Task AddAsync(Asset asset);
    void Update(Asset asset);
    Task<Asset?> GetByTickerAsync(string ticker);

    /// <summary>
    /// Garante que transações novas não fiquem com estado Modified (evita UPDATE por id inexistente).
    /// </summary>
    Task PrepareAssetForPersistenceAsync(Asset asset, CancellationToken cancellationToken = default);
}