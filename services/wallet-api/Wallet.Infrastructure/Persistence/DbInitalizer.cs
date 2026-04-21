using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wallet.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<WalletDbContext>>();
        var context = services.GetRequiredService<WalletDbContext>();

        try
        {
            logger.LogInformation("Verificando migrações pendentes...");

            // Aplica qualquer migração pendente de forma assíncrona
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogInformation("Aplicando migrações no PostgreSQL...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrações aplicadas com sucesso.");
            }
            else
            {
                logger.LogInformation("O banco de dados já está atualizado.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações.");
            throw; // Em desenvolvimento, queremos que a app pare se o banco falhar
        }
    }
}