using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        object payload;

        if (exception is FluentValidation.ValidationException validationException)
        {
            code = HttpStatusCode.BadRequest;
            payload = new { errors = validationException.Errors.Select(e => e.ErrorMessage) };
        }
        else if (exception is DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Falha ao persistir no banco");
            payload = new
            {
                error = "Falha ao salvar os dados.",
                detail = dbEx.InnerException?.Message ?? dbEx.Message
            };
        }
        else if (exception is ArgumentException or InvalidOperationException)
        {
            code = HttpStatusCode.BadRequest;
            payload = new { error = exception.Message };
        }
        else
        {
            payload = new { error = "Ocorreu um erro interno no servidor." };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}