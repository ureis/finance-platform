using MassTransit;
using Quotes.Contracts;
namespace Quotes.Api.Workers;

public class PriceSimulatorWorker(
    ILogger<PriceSimulatorWorker> logger, 
    IBus bus) : BackgroundService
{
    private readonly string[] _tickers = ["PETR4", "VALE3", "CDB_ITAU", "AAPL34"];
    private readonly Random _random = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Iniciando Simulador de Preços...");

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var ticker in _tickers)
            {
                var correlationId = Guid.NewGuid();
                var newPrice = (decimal)(_random.NextDouble() * 100);

                var @event = new AssetPriceUpdatedEvent(
                    ticker,
                    Math.Round(newPrice, 2),
                    DateTime.UtcNow);

                using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId.ToString()))
                {
                    await bus.Publish(@event, context =>
                    {
                        context.CorrelationId = correlationId;
                    }, stoppingToken);

                    logger.LogInformation("Preço publicado: {Ticker} - R$ {Price}",
                        ticker, @event.NewPrice);
                }
            }

            await Task.Delay(10000, stoppingToken); // Espera 10 segundos
        }
    }
}