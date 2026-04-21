using Wallet.Domain.Repositories;

namespace Wallet.Infrastructure.Persistence;

public class UnitOfWork(WalletDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}