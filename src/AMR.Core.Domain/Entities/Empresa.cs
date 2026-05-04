using AMR.Core.Domain.ValueObjects;

namespace AMR.Core.Domain.Entities;

/// <summary>
/// Entidade raiz do domínio ERP.
/// Representa uma empresa (CNPJ próprio) que pode ser matriz ou filial.
/// </summary>
public class Empresa
{
    public int    Id            { get; private set; }
    public string RazaoSocial  { get; private set; }
    public string NomeFantasia { get; private set; }
    public CNPJ   CNPJ         { get; private set; }
    public string? InscricaoEstadual { get; private set; }
    public string? Email        { get; private set; }
    public string? Telefone     { get; private set; }
    public Endereco? Endereco   { get; private set; }
    public bool   Ativo         { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // EF Core
    protected Empresa() { RazaoSocial = null!; NomeFantasia = null!; CNPJ = null!; }

    private Empresa(
        string razaoSocial, string nomeFantasia, CNPJ cnpj,
        string? inscricaoEstadual, string? email, string? telefone, Endereco? endereco)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException("Razão Social é obrigatória.", nameof(razaoSocial));

        if (string.IsNullOrWhiteSpace(nomeFantasia))
            throw new ArgumentException("Nome Fantasia é obrigatório.", nameof(nomeFantasia));

        ArgumentNullException.ThrowIfNull(cnpj);

        RazaoSocial       = razaoSocial.Trim();
        NomeFantasia      = nomeFantasia.Trim();
        CNPJ              = cnpj;
        InscricaoEstadual = inscricaoEstadual?.Trim();
        Email             = email?.Trim().ToLowerInvariant();
        Telefone          = telefone?.Trim();
        Endereco          = endereco;
        Ativo             = true;
        DataCadastro      = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria uma nova empresa validada.
    /// </summary>
    public static Empresa Criar(
        string razaoSocial,
        string nomeFantasia,
        CNPJ cnpj,
        string? inscricaoEstadual = null,
        string? email = null,
        string? telefone = null,
        Endereco? endereco = null)
        => new(razaoSocial, nomeFantasia, cnpj, inscricaoEstadual, email, telefone, endereco);

    // ── Comportamentos ─────────────────────────────────────────────────────────

    public void AtualizarDadosCadastrais(
        string razaoSocial, string nomeFantasia,
        string? inscricaoEstadual = null, string? email = null, string? telefone = null)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException("Razão Social é obrigatória.", nameof(razaoSocial));

        RazaoSocial       = razaoSocial.Trim();
        NomeFantasia      = nomeFantasia.Trim();
        InscricaoEstadual = inscricaoEstadual?.Trim();
        Email             = email?.Trim().ToLowerInvariant();
        Telefone          = telefone?.Trim();
        DataAtualizacao   = DateTime.UtcNow;
    }

    public void DefinirEndereco(Endereco endereco)
    {
        ArgumentNullException.ThrowIfNull(endereco);
        Endereco        = endereco;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        if (!Ativo)
            throw new InvalidOperationException("Empresa já está inativa.");

        Ativo           = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Reativar()
    {
        if (Ativo)
            throw new InvalidOperationException("Empresa já está ativa.");

        Ativo           = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public override string ToString() =>
        $"[{Id}] {NomeFantasia} — CNPJ: {CNPJ.Formatado} — {(Ativo ? "Ativa" : "Inativa")}";
}
