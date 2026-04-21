using Serilog.Context;

namespace Wallet.Api.Middleware;

/// <summary>
/// Confia no Gateway (rede privada): exige X-User-ID já validado e injeta escopo de log.
/// </summary>
public sealed class GatewayUserMiddleware(RequestDelegate next)
{
    private static readonly PathString V1Prefix = "/v1";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments(V1Prefix))
        {
            await next(context);
            return;
        }

        var userId = context.Request.Headers["X-User-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Missing X-User-ID. Chame a API apenas através do Gateway com JWT."
            });
            return;
        }

        userId = userId.Trim();
        context.Items["GatewayUserId"] = userId;

        using (LogContext.PushProperty("GatewayUserId", userId))
        {
            await next(context);
        }
    }
}
