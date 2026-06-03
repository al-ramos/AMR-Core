using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AMR.Core.Application.Interfaces;
using AMR.Core.Infrastructure.Data;
using AMR.Core.Infrastructure.Data.Repositories;

namespace AMR.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
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

        return services;
    }
}
