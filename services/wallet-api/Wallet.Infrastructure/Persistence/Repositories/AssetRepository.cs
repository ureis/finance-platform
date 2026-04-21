using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities.Assets;
using Wallet.Domain.Repositories;

namespace Wallet.Infrastructure.Persistence.Repositories;

public class AssetRepository(WalletDbContext context) : IAssetRepository
{
    public async Task<Asset?> GetByIdAsync(Guid id)
        => await context.Assets.FindAsync(id);

    public async Task<IEnumerable<Asset>> GetAllAsync()
        => await context.Assets.AsNoTracking().ToListAsync();

    public async Task AddAsync(Asset asset)
        => await context.Assets.AddAsync(asset);

    /// <summary>
    /// Só chama DbSet.Update quando o ativo não está rastreado.
    /// Update em entidade já rastreada pode gerar SQL incorreto e DbUpdateConcurrencyException (0 linhas).
    /// </summary>
    public void Update(Asset asset)
    {
        if (context.Entry(asset).State == EntityState.Detached)
        {
            context.Assets.Update(asset);
        }
    }

    public async Task<Asset?> GetByTickerAsync(string ticker)
    {
        var normalized = ticker.Trim().ToUpperInvariant();
        return await context.Assets
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Ticker == normalized);
    }

    public async Task PrepareAssetForPersistenceAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        foreach (var tx in asset.Transactions)
        {
            var entry = context.Entry(tx);
            if (entry.State != EntityState.Modified)
                continue;

            var dbValues = await entry.GetDatabaseValuesAsync(cancellationToken);
            if (dbValues is null)
                entry.State = EntityState.Added;
        }
    }
}