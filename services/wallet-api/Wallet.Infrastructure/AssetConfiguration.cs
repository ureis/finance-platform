using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities.Assets;

namespace Wallet.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("assets");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Ticker)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(a => a.Ticker)
            .IsUnique();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Type)
            .HasConversion<string>() // Guarda o Enum como String no banco
            .IsRequired();

        builder.Property(a => a.Quantity)
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(a => a.CurrentPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.OwnsOne(a => a.AveragePrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("average_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(p => p.Currency)
                .HasColumnName("average_price_currency")
                .HasDefaultValue("BRL")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}