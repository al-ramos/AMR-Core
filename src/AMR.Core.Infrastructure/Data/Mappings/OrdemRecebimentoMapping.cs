using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class OrdemRecebimentoMapping : IEntityTypeConfiguration<OrdemRecebimento>
{
    public void Configure(EntityTypeBuilder<OrdemRecebimento> b)
    {
        b.ToTable("ORDEM_RECEBIMENTO");
        b.HasKey(o => o.Id);
        b.Property(o => o.Id).HasColumnName("CD_ORDEM_RECEBIMENTO").ValueGeneratedOnAdd();
        b.Property(o => o.PedidoCompraId).HasColumnName("CD_PEDIDO_COMPRA");
        b.Property(o => o.Status).HasColumnName("CD_STATUS").HasConversion<int>();
        b.Property(o => o.DataCriacao).HasColumnName("DT_CRIACAO");
        b.Property(o => o.DataRecebimento).HasColumnName("DT_RECEBIMENTO");

        b.HasOne(o => o.PedidoCompra).WithMany().HasForeignKey(o => o.PedidoCompraId);

        b.HasMany(o => o.Itens).WithOne()
            .HasForeignKey(i => i.OrdemRecebimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(o => o.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ItemRecebimentoMapping : IEntityTypeConfiguration<ItemRecebimento>
{
    public void Configure(EntityTypeBuilder<ItemRecebimento> b)
    {
        b.ToTable("ITEM_RECEBIMENTO");
        b.HasKey(i => i.Id);
        b.Property(i => i.Id).HasColumnName("CD_ITEM").ValueGeneratedOnAdd();
        b.Property(i => i.OrdemRecebimentoId).HasColumnName("CD_ORDEM_RECEBIMENTO");
        b.Property(i => i.ProdutoId).HasColumnName("CD_PRODUTO");
        b.Property(i => i.LocalizacaoId).HasColumnName("CD_LOCALIZACAO");
        b.Property(i => i.QntEsperada).HasColumnName("QT_ESPERADA").HasPrecision(18, 4);
        b.Property(i => i.QntRecebida).HasColumnName("QT_RECEBIDA").HasPrecision(18, 4);

        b.HasOne(i => i.Produto).WithMany().HasForeignKey(i => i.ProdutoId);
    }
}
