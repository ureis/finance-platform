using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Domain.Repositories;
using Wallet.Infrastructure.Persistence;
using Wallet.Infrastructure.Persistence.Repositories;

namespace Wallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<WalletDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // RESILIÊNCIA: Tenta reconectar se o banco estiver subindo (Docker)
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            })
            .UseSnakeCaseNamingConvention());

        // Registros dos Repositórios
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}