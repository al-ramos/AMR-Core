using MediatR;
using AMR.Core.Infrastructure;
using AMR.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AMR.Core API", Version = "v1" });
});

// Application — MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(AMR.Core.Application.Produtos.Commands.CriarProdutoCommand).Assembly));

// Infrastructure — DbContext + Repositórios
builder.Services.AddInfrastructure(builder.Configuration);

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

// CORS — permite o rds-forms-fabrica chamar esta API
builder.Services.AddCors(opts =>
    opts.AddPolicy("RdsFabrica", policy =>
        policy.WithOrigins(
                builder.Configuration["Cors:AllowedOrigins"] ?? "*")
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();

// Auto-migrate + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AmrCoreDbContext>();
    db.Database.Migrate();
    await AmrCoreSeed.AplicarAsync(db);
}


app.UseSwagger();
app.UseSwaggerUI();

// Redirect raiz para Swagger em dev
if (app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();

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

app.UseCors("RdsFabrica");
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

app.Run();
