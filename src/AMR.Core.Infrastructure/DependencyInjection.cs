using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using AMR.Core.Application.Interfaces;
using AMR.Core.Infrastructure.Data;
using AMR.Core.Infrastructure.Data.Repositories;
using AMR.Core.Infrastructure.ExternalServices;

namespace AMR.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        services.AddDbContext<AmrCoreDbContext>(opts =>
            opts.UseSqlite(
                configuration.GetConnectionString("AmrCore"),
                sql => sql.MigrationsAssembly(typeof(AmrCoreDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoCompraRepository, PedidoCompraRepository>();
        services.AddScoped<IPedidoVendaRepository, PedidoVendaRepository>();
        services.AddScoped<ISaldoEstoqueRepository, SaldoEstoqueRepository>();
        services.AddScoped<IMovimentoEstoqueRepository, MovimentoEstoqueRepository>();
        services.AddScoped<IOrdemRecebimentoRepository, OrdemRecebimentoRepository>();

        // ── TMS API Client ─────────────────────────────────────────────────────
        if (env.IsDevelopment())
        {
            services.AddSingleton<ITmsApiClient, LocalTmsApiClient>();
        }
        else
        {
            services
                .AddHttpClient<ITmsApiClient, TmsApiClient>(client =>
                {
                    client.BaseAddress = new Uri(
                        configuration["TmsApi:BaseUrl"] ?? "http://localhost:3002");
                    client.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
        }

        // ── Compras API Client ────────────────────────────────────────────────
        if (env.IsDevelopment())
        {
            services.AddSingleton<IComprasApiClient, LocalComprasApiClient>();
        }
        else
        {
            services
                .AddHttpClient<IComprasApiClient, ComprasApiClient>(client =>
                {
                    client.BaseAddress = new Uri(
                        configuration["ComprasApi:BaseUrl"] ?? "http://localhost:3001");
                    client.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
        }

        return services;
    }
}
