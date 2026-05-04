using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.ValueObjects;

namespace AMR.Core.Infrastructure.Data.Mappings;

public class FornecedorMapping : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> b)
    {
        b.ToTable("FORNECEDOR");
        b.HasKey(f => f.Id);
        b.Property(f => f.Id).HasColumnName("CD_FORNECEDOR").ValueGeneratedOnAdd();
        b.Property(f => f.EmpresaId).HasColumnName("CD_EMPRESA");
        b.Property(f => f.RazaoSocial).HasColumnName("NM_RAZAO_SOCIAL").HasMaxLength(200).IsRequired();
        b.Property(f => f.NomeFantasia).HasColumnName("NM_FANTASIA").HasMaxLength(150).IsRequired();
        b.Property(f => f.Categoria).HasColumnName("DS_CATEGORIA").HasMaxLength(100).IsRequired();
        b.Property(f => f.InscricaoEstadual).HasColumnName("NR_IE").HasMaxLength(30);
        b.Property(f => f.Email).HasColumnName("DS_EMAIL").HasMaxLength(150);
        b.Property(f => f.Telefone).HasColumnName("NR_TELEFONE").HasMaxLength(20);
        b.Property(f => f.ContatoResponsavel).HasColumnName("NM_CONTATO").HasMaxLength(150);
        b.Property(f => f.Ativo).HasColumnName("ST_ATIVO").HasDefaultValue(true);
        b.Property(f => f.DataCadastro).HasColumnName("DT_CADASTRO");
        b.Property(f => f.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");

        b.Property(f => f.CNPJ)
            .HasConversion(
                cnpj => cnpj.Numero,
                str  => CNPJ.Criar(str))
            .HasColumnName("NR_CNPJ").HasMaxLength(14).IsRequired();

        b.OwnsOne(f => f.Endereco, e =>
        {
            e.Property(x => x.Logradouro).HasColumnName("DS_LOGRADOURO").HasMaxLength(200);
            e.Property(x => x.Numero).HasColumnName("NR_NUMERO").HasMaxLength(20);
            e.Property(x => x.Complemento).HasColumnName("DS_COMPLEMENTO").HasMaxLength(100);
            e.Property(x => x.Bairro).HasColumnName("DS_BAIRRO").HasMaxLength(100);
            e.Property(x => x.Cidade).HasColumnName("DS_CIDADE").HasMaxLength(100);
            e.Property(x => x.Estado).HasColumnName("DS_ESTADO").HasMaxLength(2);
            e.Property(x => x.CEP).HasColumnName("NR_CEP").HasMaxLength(8);
            e.Property(x => x.Pais).HasColumnName("DS_PAIS").HasMaxLength(50);
        });
    }
}
