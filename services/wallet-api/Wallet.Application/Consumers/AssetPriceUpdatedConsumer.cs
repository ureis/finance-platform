using MassTransit;
using Microsoft.Extensions.Logging;
using Quotes.Contracts;
using Wallet.Domain.Repositories;

namespace Wallet.Application.Consumers;

public class AssetPriceUpdatedConsumer(
    IAssetRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<AssetPriceUpdatedConsumer> logger) : IConsumer<AssetPriceUpdatedEvent>
{
    public async Task Consume(ConsumeContext<AssetPriceUpdatedEvent> context)
    {
        var correlationId =
            context.CorrelationId?.ToString()
            ?? context.ConversationId?.ToString()
            ?? Guid.NewGuid().ToString();

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            var message = context.Message;

            // 1. Busca o ativo no banco pelo Ticker
            var asset = await repository.GetByTickerAsync(message.Ticker);

            if (asset == null)
            {
                return;
            }

            logger.LogInformation("--- EVENTO RECEBIDO: Atualizando {Ticker} para R$ {Price}",
                message.Ticker, message.NewPrice);

            asset.UpdateCurrentPrice(message.NewPrice);

            repository.Update(asset);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
