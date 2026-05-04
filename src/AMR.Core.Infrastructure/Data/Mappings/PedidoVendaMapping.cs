using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class PedidoVendaMapping : IEntityTypeConfiguration<PedidoVenda>
{
    public void Configure(EntityTypeBuilder<PedidoVenda> b)
    {
        b.ToTable("PEDIDO_VENDA");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).HasColumnName("CD_PEDIDO_VENDA").ValueGeneratedOnAdd();
        b.Property(p => p.EmpresaId).HasColumnName("CD_EMPRESA");
        b.Property(p => p.ClienteId).HasColumnName("CD_CLIENTE");
        b.Property(p => p.Status).HasColumnName("CD_STATUS").HasConversion<int>();
        b.Property(p => p.DataEmissao).HasColumnName("DT_EMISSAO");
        b.Property(p => p.DataAprovacao).HasColumnName("DT_APROVACAO");
        b.Property(p => p.DataFaturamento).HasColumnName("DT_FATURAMENTO");
        b.Property(p => p.Observacao).HasColumnName("DS_OBSERVACAO").HasMaxLength(500);

        b.HasMany(p => p.Itens).WithOne()
            .HasForeignKey(nameof(ItemPedidoVenda) + "Id")
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(p => p.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ItemPedidoVendaMapping : IEntityTypeConfiguration<ItemPedidoVenda>
{
    public void Configure(EntityTypeBuilder<ItemPedidoVenda> b)
    {
        b.ToTable("ITEM_PEDIDO_VENDA");
        b.HasKey(i => i.Id);
        b.Property(i => i.Id).HasColumnName("CD_ITEM").ValueGeneratedOnAdd();
        b.Property(i => i.PedidoVendaId).HasColumnName("CD_PEDIDO_VENDA");
        b.Property(i => i.ProdutoId).HasColumnName("CD_PRODUTO");
        b.Property(i => i.Quantidade).HasColumnName("QT_QUANTIDADE").HasPrecision(18, 4);
        b.Property(i => i.PrecoUnitario).HasColumnName("VL_PRECO_UNITARIO").HasPrecision(18, 4);
        b.Property(i => i.Desconto).HasColumnName("PC_DESCONTO").HasPrecision(5, 2);
        b.Ignore(i => i.Total);
        b.HasOne(i => i.Produto).WithMany().HasForeignKey(i => i.ProdutoId);
    }
}
