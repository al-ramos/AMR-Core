using Microsoft.EntityFrameworkCore;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data;

public class AmrCoreDbContext(DbContextOptions<AmrCoreDbContext> options) : DbContext(options)
{
    public DbSet<Empresa>           Empresas           => Set<Empresa>();
    public DbSet<Cliente>           Clientes           => Set<Cliente>();
    public DbSet<Fornecedor>        Fornecedores       => Set<Fornecedor>();
    public DbSet<UnidadeMedida>     UnidadesMedida     => Set<UnidadeMedida>();
    public DbSet<Produto>           Produtos           => Set<Produto>();
    public DbSet<PedidoCompra>      PedidosCompra      => Set<PedidoCompra>();
    public DbSet<ItemPedidoCompra>  ItensPedidoCompra  => Set<ItemPedidoCompra>();
    public DbSet<PedidoVenda>       PedidosVenda       => Set<PedidoVenda>();
    public DbSet<ItemPedidoVenda>   ItensPedidoVenda   => Set<ItemPedidoVenda>();
    public DbSet<SaldoEstoque>      SaldosEstoque      => Set<SaldoEstoque>();
    public DbSet<MovimentoEstoque>  MovimentosEstoque  => Set<MovimentoEstoque>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(AmrCoreDbContext).Assembly);
        base.OnModelCreating(mb);
    }
}
