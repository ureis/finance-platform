using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;
using Wallet.Domain.Entities.Assets;

namespace Wallet.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(t => t.Id);

        // IDs são definidos no domínio (Guid.NewGuid); evita confusão do EF com ValueGeneratedOnAdd.
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.AssetId).IsRequired();

        builder.Property(t => t.Quantity).HasPrecision(18, 8);

        builder.OwnsOne(t => t.PriceAtTime, price =>
        {
            price.Property(p => p.Amount).HasColumnName("price_amount").HasPrecision(18, 2);
            price.Property(p => p.Currency).HasColumnName("price_currency").HasMaxLength(3);
        });

        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasOne<Asset>()
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}