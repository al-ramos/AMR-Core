using Microsoft.AspNetCore.Mvc;

namespace AMR.Core.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exceção não tratada em {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
            await HandleAsync(ctx, ex);
        }
    }

    private Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, title) = ex switch
        {
            ArgumentException         => (StatusCodes.Status400BadRequest,          "Requisição inválida"),
            InvalidOperationException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
            KeyNotFoundException      => (StatusCodes.Status404NotFound,            "Não encontrado"),
            _                         => (StatusCodes.Status500InternalServerError, "Erro interno do servidor"),
        };

        var detail = env.IsProduction() && status == 500
            ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
            : ex.Message;

        var problem = new ProblemDetails
        {
            Status   = status,
            Title    = title,
            Detail   = detail,
            Instance = ctx.Request.Path,
        };

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode  = status;
        return ctx.Response.WriteAsJsonAsync(problem);
    }
}
