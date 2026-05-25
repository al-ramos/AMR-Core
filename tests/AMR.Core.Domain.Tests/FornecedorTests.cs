using AMR.Core.Domain.Entities;
using AMR.Core.Domain.ValueObjects;
using Xunit;

namespace AMR.Core.Domain.Tests;

public class FornecedorTests
{
    private static CNPJ CriarCNPJValido() => CNPJ.Criar("11222333000181");

    [Fact]
    public void Criar_FornecedorValido_DeveCriarComSucesso()
    {
        // Arrange
        var empresaId = 1;
        var razaoSocial = "Empresa XYZ Ltda";
        var nomeFantasia = "XYZ";
        var cnpj = CriarCNPJValido();
        var categoria = "Serviços";

        // Act
        var fornecedor = Fornecedor.Criar(empresaId, razaoSocial, nomeFantasia, cnpj, categoria);

        // Assert
        Assert.NotNull(fornecedor);
        Assert.Equal(nomeFantasia, fornecedor.NomeFantasia);
        Assert.True(fornecedor.Ativo);
    }

    [Fact]
    public void Criar_SemNomeFantasia_DeveLancarExcecao()
    {
        var cnpj = CriarCNPJValido();
        
        Assert.Throws<ArgumentException>(() =>
            Fornecedor.Criar(1, "Empresa", "", cnpj, "Serviços"));
    }

    [Fact]
    public void Inativar_FornecedorAtivo_DeveInatiar()
    {
        // Arrange
        var fornecedor = Fornecedor.Criar(1, "Empresa", "XYZ", CriarCNPJValido(), "Serviços");
        Assert.True(fornecedor.Ativo);

        // Act
        fornecedor.Inativar();

        // Assert
        Assert.False(fornecedor.Ativo);
    }

    [Fact]
    public void Reativar_FornecedorInativo_DeveReativar()
    {
        // Arrange
        var fornecedor = Fornecedor.Criar(1, "Empresa", "XYZ", CriarCNPJValido(), "Serviços");
        fornecedor.Inativar();
        Assert.False(fornecedor.Ativo);

        // Act
        fornecedor.Reativar();

        // Assert
        Assert.True(fornecedor.Ativo);
    }
}
