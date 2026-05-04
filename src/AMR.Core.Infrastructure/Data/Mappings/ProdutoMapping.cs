using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> b)
    {
        b.ToTable("PRODUTO");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).HasColumnName("CD_PRODUTO").ValueGeneratedOnAdd();
        b.Property(p => p.SKU).HasColumnName("CD_SKU").HasMaxLength(50).IsRequired();
        b.Property(p => p.Nome).HasColumnName("NM_PRODUTO").HasMaxLength(200).IsRequired();
        b.Property(p => p.Descricao).HasColumnName("DS_PRODUTO").HasMaxLength(500);
        b.Property(p => p.PrecoUnitario).HasColumnName("VL_PRECO_UNITARIO").HasPrecision(18, 4);
        b.Property(p => p.EstoqueMinimo).HasColumnName("QT_ESTOQUE_MINIMO").HasPrecision(18, 4);
        b.Property(p => p.UnidadeMedidaId).HasColumnName("CD_UNIDADE_MEDIDA");
        b.Property(p => p.Ativo).HasColumnName("ST_ATIVO").HasDefaultValue(true);
        b.Property(p => p.DataCadastro).HasColumnName("DT_CADASTRO");
        b.Property(p => p.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
        b.HasOne(p => p.UnidadeMedida).WithMany().HasForeignKey(p => p.UnidadeMedidaId);
        b.HasIndex(p => p.SKU).IsUnique();
    }
}
