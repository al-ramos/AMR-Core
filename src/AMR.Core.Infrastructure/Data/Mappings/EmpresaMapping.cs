using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.ValueObjects;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class EmpresaMapping : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> b)
    {
        b.ToTable("EMPRESA");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("CD_EMPRESA").ValueGeneratedOnAdd();
        b.Property(e => e.RazaoSocial).HasColumnName("NM_RAZAO_SOCIAL").HasMaxLength(200).IsRequired();
        b.Property(e => e.NomeFantasia).HasColumnName("NM_FANTASIA").HasMaxLength(150).IsRequired();
        b.Property(e => e.InscricaoEstadual).HasColumnName("NR_IE").HasMaxLength(30);
        b.Property(e => e.Email).HasColumnName("DS_EMAIL").HasMaxLength(150);
        b.Property(e => e.Telefone).HasColumnName("NR_TELEFONE").HasMaxLength(20);
        b.Property(e => e.Ativo).HasColumnName("ST_ATIVO").HasDefaultValue(true);
        b.Property(e => e.DataCadastro).HasColumnName("DT_CADASTRO");
        b.Property(e => e.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");

        b.Property(e => e.CNPJ)
            .HasConversion(
                cnpj => cnpj.Numero,
                str  => CNPJ.Criar(str))
            .HasColumnName("NR_CNPJ").HasMaxLength(14).IsRequired();

        b.OwnsOne(e => e.Endereco, end =>
        {
            end.Property(x => x.Logradouro).HasColumnName("DS_LOGRADOURO").HasMaxLength(200);
            end.Property(x => x.Numero).HasColumnName("NR_NUMERO").HasMaxLength(20);
            end.Property(x => x.Complemento).HasColumnName("DS_COMPLEMENTO").HasMaxLength(100);
            end.Property(x => x.Bairro).HasColumnName("DS_BAIRRO").HasMaxLength(100);
            end.Property(x => x.Cidade).HasColumnName("DS_CIDADE").HasMaxLength(100);
            end.Property(x => x.Estado).HasColumnName("DS_ESTADO").HasMaxLength(2);
            end.Property(x => x.CEP).HasColumnName("NR_CEP").HasMaxLength(8);
            end.Property(x => x.Pais).HasColumnName("DS_PAIS").HasMaxLength(50);
        });
    }
}
