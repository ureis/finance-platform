using Serilog.Context;

namespace Wallet.Api.Middleware;

public class TracingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Captura o ID que o Go enviou
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        // LogContext.PushProperty faz com que TODOS os logs dentro desta requisição
        // incluam automaticamente a coluna "CorrelationId"
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.Headers.Append("X-Correlation-ID", correlationId);
            await next(context);
        }
    }
}