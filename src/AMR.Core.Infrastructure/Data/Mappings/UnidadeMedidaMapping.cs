using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class UnidadeMedidaMapping : IEntityTypeConfiguration<UnidadeMedida>
{
    public void Configure(EntityTypeBuilder<UnidadeMedida> b)
    {
        b.ToTable("UNIDADE_MEDIDA");
        b.HasKey(u => u.Id);
        b.Property(u => u.Id).HasColumnName("CD_UNIDADE").ValueGeneratedOnAdd();
        b.Property(u => u.Sigla).HasColumnName("CD_SIGLA").HasMaxLength(10).IsRequired();
        b.Property(u => u.Descricao).HasColumnName("DS_UNIDADE").HasMaxLength(100).IsRequired();
        b.Property(u => u.Ativo).HasColumnName("ST_ATIVO").HasDefaultValue(true);
        b.HasIndex(u => u.Sigla).IsUnique();
    }
}
