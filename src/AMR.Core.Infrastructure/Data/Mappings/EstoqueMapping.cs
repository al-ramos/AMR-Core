using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class SaldoEstoqueMapping : IEntityTypeConfiguration<SaldoEstoque>
{
    public void Configure(EntityTypeBuilder<SaldoEstoque> b)
    {
        b.ToTable("SALDO_ESTOQUE");
        b.HasKey(s => s.Id);
        b.Property(s => s.Id).HasColumnName("CD_SALDO").ValueGeneratedOnAdd();
        b.Property(s => s.ProdutoId).HasColumnName("CD_PRODUTO");
        b.Property(s => s.EmpresaId).HasColumnName("CD_EMPRESA");
        b.Property(s => s.Quantidade).HasColumnName("QT_SALDO").HasPrecision(18, 4);
        b.HasOne(s => s.Produto).WithMany().HasForeignKey(s => s.ProdutoId);
        b.HasIndex(s => new { s.ProdutoId, s.EmpresaId }).IsUnique();
    }
}

public class MovimentoEstoqueMapping : IEntityTypeConfiguration<MovimentoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentoEstoque> b)
    {
        b.ToTable("MOVIMENTO_ESTOQUE");
        b.HasKey(m => m.Id);
        b.Property(m => m.Id).HasColumnName("CD_MOVIMENTO").ValueGeneratedOnAdd();
        b.Property(m => m.ProdutoId).HasColumnName("CD_PRODUTO");
        b.Property(m => m.EmpresaId).HasColumnName("CD_EMPRESA");
        b.Property(m => m.Tipo).HasColumnName("CD_TIPO").HasConversion<int>();
        b.Property(m => m.Quantidade).HasColumnName("QT_QUANTIDADE").HasPrecision(18, 4);
        b.Property(m => m.Origem).HasColumnName("DS_ORIGEM").HasMaxLength(50);
        b.Property(m => m.DataHora).HasColumnName("DT_MOVIMENTO");
        b.HasOne(m => m.Produto).WithMany().HasForeignKey(m => m.ProdutoId);
    }
}
