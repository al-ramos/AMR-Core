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
app.UseCors("RdsFabrica");
app.UseAuthorization();
app.MapControllers();

app.Run();
