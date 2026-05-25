using AMR.Core.Domain.Entities;
using AMR.Core.Domain.ValueObjects;
using Xunit;

namespace AMR.Core.Domain.Tests;

public class EmpresaTests
{
    private static CNPJ CriarCNPJValido() => CNPJ.Criar("11222333000181");

    [Fact]
    public void Criar_EmpresaValida_DeveCriarComSucesso()
    {
        // Arrange
        var razaoSocial = "Empresa XYZ Ltda";
        var nomeFantasia = "XYZ";
        var cnpj = CriarCNPJValido();

        // Act
        var empresa = Empresa.Criar(razaoSocial, nomeFantasia, cnpj);

        // Assert
        Assert.NotNull(empresa);
        Assert.Equal(razaoSocial.Trim(), empresa.RazaoSocial);
        Assert.Equal(nomeFantasia.Trim(), empresa.NomeFantasia);
        Assert.True(empresa.Ativo);
    }

    [Fact]
    public void Criar_ComTodosOsDados_DeveCriarCompleto()
    {
        // Arrange
        var razaoSocial = "Empresa XYZ Ltda";
        var nomeFantasia = "XYZ";
        var cnpj = CriarCNPJValido();
        var email = "contato@xyz.com";
        var telefone = "(11) 98765-4321";
        var inscricaoEstadual = "123.456.789.012";

        // Act
        var empresa = Empresa.Criar(razaoSocial, nomeFantasia, cnpj, inscricaoEstadual, email, telefone);

        // Assert
        Assert.NotNull(empresa);
        Assert.Equal(email.ToLowerInvariant(), empresa.Email);
        Assert.Equal(telefone.Trim(), empresa.Telefone);
        Assert.Equal(inscricaoEstadual.Trim(), empresa.InscricaoEstadual);
    }

    [Fact]
    public void Criar_SemRazaoSocial_DeveLancarExcecao()
    {
        var cnpj = CriarCNPJValido();
        
        Assert.Throws<ArgumentException>(() =>
            Empresa.Criar("", "XYZ", cnpj));
    }

    [Fact]
    public void Criar_SemNomeFantasia_DeveLancarExcecao()
    {
        var cnpj = CriarCNPJValido();
        
        Assert.Throws<ArgumentException>(() =>
            Empresa.Criar("Empresa XYZ", "", cnpj));
    }

    [Fact]
    public void Criar_SemCNPJ_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Empresa.Criar("Empresa XYZ", "XYZ", null!));
    }

    [Fact]
    public void AtualizarDadosCadastrais_ComNovosDados_DeveAtualizar()
    {
        // Arrange
        var empresa = Empresa.Criar("Empresa XYZ", "XYZ", CriarCNPJValido());
        var novoEmail = "novo@xyz.com";
        var novoTelefone = "(21) 91234-5678";

        // Act
        empresa.AtualizarDadosCadastrais("Empresa XYZ Nova", "XYZ Nova", "999.999.999/9999-99", novoEmail, novoTelefone);

        // Assert
        Assert.Equal(novoEmail.ToLowerInvariant(), empresa.Email);
        Assert.Equal(novoTelefone.Trim(), empresa.Telefone);
    }

    [Fact]
    public void DataCadastro_AoPrimeiraInstancia_DeveSerHoje()
    {
        // Arrange
        var agora = DateTime.UtcNow;
        
        // Act
        var empresa = Empresa.Criar("Empresa XYZ", "XYZ", CriarCNPJValido());

        // Assert
        Assert.True(empresa.DataCadastro <= agora.AddSeconds(1));
        Assert.True(empresa.DataCadastro >= agora.AddSeconds(-5));
    }

    [Fact]
    public void Email_AoSalvar_DeveSerEmMinusculas()
    {
        // Arrange
        var emailMaisculas = "CONTATO@XYZ.COM";
        
        // Act
        var empresa = Empresa.Criar("Empresa XYZ", "XYZ", CriarCNPJValido(), 
            email: emailMaisculas);

        // Assert
        Assert.Equal(emailMaisculas.ToLowerInvariant(), empresa.Email);
    }
}
