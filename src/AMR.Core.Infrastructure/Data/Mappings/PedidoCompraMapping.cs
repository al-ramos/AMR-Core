using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class PedidoCompraMapping : IEntityTypeConfiguration<PedidoCompra>
{
    public void Configure(EntityTypeBuilder<PedidoCompra> b)
    {
        b.ToTable("PEDIDO_COMPRA");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).HasColumnName("CD_PEDIDO_COMPRA").ValueGeneratedOnAdd();
        b.Property(p => p.EmpresaId).HasColumnName("CD_EMPRESA");
        b.Property(p => p.FornecedorId).HasColumnName("CD_FORNECEDOR");
        b.Property(p => p.Status).HasColumnName("CD_STATUS").HasConversion<int>();
        b.Property(p => p.DataEmissao).HasColumnName("DT_EMISSAO");
        b.Property(p => p.DataAprovacao).HasColumnName("DT_APROVACAO");
        b.Property(p => p.DataRecebimento).HasColumnName("DT_RECEBIMENTO");
        b.Property(p => p.Observacao).HasColumnName("DS_OBSERVACAO").HasMaxLength(500);

        b.HasMany(p => p.Itens).WithOne()
            .HasForeignKey(nameof(ItemPedidoCompra) + "Id")
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(p => p.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ItemPedidoCompraMapping : IEntityTypeConfiguration<ItemPedidoCompra>
{
    public void Configure(EntityTypeBuilder<ItemPedidoCompra> b)
    {
        b.ToTable("ITEM_PEDIDO_COMPRA");
        b.HasKey(i => i.Id);
        b.Property(i => i.Id).HasColumnName("CD_ITEM").ValueGeneratedOnAdd();
        b.Property(i => i.PedidoCompraId).HasColumnName("CD_PEDIDO_COMPRA");
        b.Property(i => i.ProdutoId).HasColumnName("CD_PRODUTO");
        b.Property(i => i.Quantidade).HasColumnName("QT_QUANTIDADE").HasPrecision(18, 4);
        b.Property(i => i.PrecoUnitario).HasColumnName("VL_PRECO_UNITARIO").HasPrecision(18, 4);
        b.Ignore(i => i.Total);
        b.HasOne(i => i.Produto).WithMany().HasForeignKey(i => i.ProdutoId);
    }
}
