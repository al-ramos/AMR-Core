using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.ValueObjects;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("CLIENTE");
        b.HasKey(c => c.Id);
        b.Property(c => c.Id).HasColumnName("CD_CLIENTE").ValueGeneratedOnAdd();
        b.Property(c => c.EmpresaId).HasColumnName("CD_EMPRESA");
        b.Property(c => c.Nome).HasColumnName("NM_CLIENTE").HasMaxLength(200).IsRequired();
        b.Property(c => c.TipoDocumento).HasColumnName("TP_DOCUMENTO").HasMaxLength(4).IsRequired();
        b.Property(c => c.NumeroDocumento).HasColumnName("NR_DOCUMENTO").HasMaxLength(14).IsRequired();
        b.Property(c => c.Email).HasColumnName("DS_EMAIL").HasMaxLength(150);
        b.Property(c => c.Telefone).HasColumnName("NR_TELEFONE").HasMaxLength(20);
        b.Property(c => c.Ativo).HasColumnName("ST_ATIVO").HasDefaultValue(true);
        b.Property(c => c.DataCadastro).HasColumnName("DT_CADASTRO");
        b.Property(c => c.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");

        b.Property(c => c.CNPJ)
            .HasConversion(
                cnpj => cnpj == null ? null : cnpj.Numero,
                str  => str  == null ? null : CNPJ.Criar(str))
            .HasColumnName("NR_CNPJ").HasMaxLength(14);

        b.OwnsOne(c => c.Endereco, MapEndereco);
    }

    internal static void MapEndereco(OwnedNavigationBuilder<Cliente, Endereco> e)
    {
        e.Property(x => x.Logradouro).HasColumnName("DS_LOGRADOURO").HasMaxLength(200);
        e.Property(x => x.Numero).HasColumnName("NR_NUMERO").HasMaxLength(20);
        e.Property(x => x.Complemento).HasColumnName("DS_COMPLEMENTO").HasMaxLength(100);
        e.Property(x => x.Bairro).HasColumnName("DS_BAIRRO").HasMaxLength(100);
        e.Property(x => x.Cidade).HasColumnName("DS_CIDADE").HasMaxLength(100);
        e.Property(x => x.Estado).HasColumnName("DS_ESTADO").HasMaxLength(2);
        e.Property(x => x.CEP).HasColumnName("NR_CEP").HasMaxLength(8);
        e.Property(x => x.Pais).HasColumnName("DS_PAIS").HasMaxLength(50);
    }
}
