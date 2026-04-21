using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;
using Wallet.Domain.Entities.Assets;
using Wallet.Infrastructure.Persistence.Configurations;

namespace Wallet.Infrastructure.Persistence;

public class WalletDbContext(DbContextOptions<WalletDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as configurações que implementam IEntityTypeConfiguration neste assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}