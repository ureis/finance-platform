using Serilog.Context;

namespace Quotes.Api.Middleware;

public sealed class TracingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.Headers.Append("X-Correlation-ID", correlationId);
            await next(context);
        }
    }
}
