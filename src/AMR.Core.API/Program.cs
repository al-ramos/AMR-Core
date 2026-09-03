using FluentValidation;
using MediatR;
using AMR.Core.Application.Behaviors;
using AMR.Core.Infrastructure;
using AMR.Core.Infrastructure.Data;
using AMR.Core.API.Telemetry;
using AMR.Core.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog como provider de log ──────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AMR.Core.API")
    .WriteTo.Console(
        outputTemplate: ctx.HostingEnvironment.IsProduction()
            ? "[{Timestamp:o} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"
            : "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AMR.Core API", Version = "v1" });
});

// Application — MediatR + ValidationBehavior
var appAssembly = typeof(AMR.Core.Application.Produtos.Commands.CriarProdutoCommand).Assembly;
builder.Services.AddValidatorsFromAssembly(appAssembly);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(appAssembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Infrastructure — DbContext + Repositórios + TmsApiClient
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// Observabilidade — OpenTelemetry
builder.Services.AddAmrTelemetry(builder.Configuration, "amr-core");

// ── Rate Limiting — 100 req/min por IP ────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
            }));
    options.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.Headers.RetryAfter = "60";
        await ctx.HttpContext.Response.WriteAsync("Too many requests. Retry after 60 seconds.", ct);
    };
});

// CORS — as origens vêm de Cors:AllowedOrigins, aceito como string única
// ou como array. Nenhuma origem fica fixada no código: o ALB
// rds-forms-fabrica-alb-558362351, que estava aqui, não existe mais na conta.
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? (builder.Configuration["Cors:AllowedOrigins"] is { Length: > 0 } origemUnica
        ? new[] { origemUnica }
        : Array.Empty<string>());

builder.Services.AddCors(opts =>
    opts.AddPolicy("AmrCore", policy =>
    {
        // Sem origem configurada a política não libera nenhuma. O fallback
        // anterior era WithOrigins("*"), que o ASP.NET Core trata como a
        // origem literal "*" — nunca liberou nada de fato.
        if (corsOrigins.Length > 0)
            policy.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader();
    }));

var app = builder.Build();

// Auto-migrate + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AmrCoreDbContext>();

    if (app.Environment.IsDevelopment())
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
    else
    {
        await db.Database.MigrateAsync();
    }

    await AmrCoreSeed.AplicarAsync(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// Redirect raiz para Swagger em dev
if (app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();

// ── Serilog request logging ───────────────────────────────────────────────────
app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.000}ms)";
});

// ── Security Headers (OWASP) ──────────────────────────────────────────────────
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"]  = "nosniff";
    ctx.Response.Headers["X-Frame-Options"]         = "DENY";
    ctx.Response.Headers["X-XSS-Protection"]        = "1; mode=block";
    ctx.Response.Headers["Referrer-Policy"]         = "strict-origin-when-cross-origin";
    ctx.Response.Headers["Permissions-Policy"]      = "geolocation=(), microphone=(), camera=()";
    if (!ctx.Request.IsHttps && app.Environment.IsProduction())
        ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    await next();
});

app.UseCors("AmrCore");
app.UseRateLimiter();
app.UseAuthorization();
// Health checks — o target group do ALB precisa de um caminho que responda sem
// depender de nada. /health e liveness pura (o processo subiu); /health/ready
// verifica o banco, que e a unica dependencia externa da API hoje.
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .ExcludeFromDescription();

app.MapGet("/health/ready", async (IServiceProvider sp, CancellationToken ct) =>
{
    try
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AmrCoreDbContext>();
        await db.Database.CanConnectAsync(ct);
        return Results.Ok(new { status = "ready" });
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "degraded", detail = ex.Message }, statusCode: 503);
    }
}).ExcludeFromDescription();

app.MapControllers();

app.Run();
