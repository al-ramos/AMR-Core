using AMR.Core.Domain.ValueObjects;

namespace AMR.Core.Domain.Entities;

/// <summary>
/// Entidade Fornecedor do domínio ERP.
/// Representa empresas que fornecem produtos ou serviços.
/// </summary>
public class Fornecedor
{
    public int      Id           { get; private set; }
    public string   RazaoSocial  { get; private set; }
    public string   NomeFantasia { get; private set; }
    public CNPJ     CNPJ         { get; private set; }
    public string?  InscricaoEstadual { get; private set; }
    public string   Categoria    { get; private set; }   // ex: "Matéria-Prima", "Serviços", "Transportadora"
    public string?  Email        { get; private set; }
    public string?  Telefone     { get; private set; }
    public string?  ContatoResponsavel { get; private set; }
    public Endereco? Endereco    { get; private set; }
    public bool     Ativo        { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Vínculo com a empresa compradora
    public int     EmpresaId  { get; private set; }
    public Empresa? Empresa   { get; private set; }

    // EF Core
    protected Fornecedor() { RazaoSocial = null!; NomeFantasia = null!; CNPJ = null!; Categoria = null!; }

    private Fornecedor(
        int empresaId, string razaoSocial, string nomeFantasia, CNPJ cnpj,
        string categoria, string? inscricaoEstadual,
        string? email, string? telefone, string? contatoResponsavel, Endereco? endereco)
    {
        if (empresaId <= 0)
            throw new ArgumentException("EmpresaId inválido.", nameof(empresaId));

        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException("Razão Social é obrigatória.", nameof(razaoSocial));

        if (string.IsNullOrWhiteSpace(nomeFantasia))
            throw new ArgumentException("Nome Fantasia é obrigatório.", nameof(nomeFantasia));

        if (string.IsNullOrWhiteSpace(categoria))
            throw new ArgumentException("Categoria é obrigatória.", nameof(categoria));

        ArgumentNullException.ThrowIfNull(cnpj);

        EmpresaId          = empresaId;
        RazaoSocial        = razaoSocial.Trim();
        NomeFantasia       = nomeFantasia.Trim();
        CNPJ               = cnpj;
        Categoria          = categoria.Trim();
        InscricaoEstadual  = inscricaoEstadual?.Trim();
        Email              = email?.Trim().ToLowerInvariant();
        Telefone           = telefone?.Trim();
        ContatoResponsavel = contatoResponsavel?.Trim();
        Endereco           = endereco;
        Ativo              = true;
        DataCadastro       = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria um novo Fornecedor validado.
    /// </summary>
    public static Fornecedor Criar(
        int empresaId,
        string razaoSocial,
        string nomeFantasia,
        CNPJ cnpj,
        string categoria,
        string? inscricaoEstadual = null,
        string? email = null,
        string? telefone = null,
        string? contatoResponsavel = null,
        Endereco? endereco = null)
        => new(empresaId, razaoSocial, nomeFantasia, cnpj, categoria,
               inscricaoEstadual, email, telefone, contatoResponsavel, endereco);

    // ── Comportamentos ─────────────────────────────────────────────────────────

    public void AtualizarCategoria(string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
            throw new ArgumentException("Categoria é obrigatória.", nameof(categoria));

        Categoria       = categoria.Trim();
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarContato(string? email, string? telefone, string? contatoResponsavel)
    {
        Email              = email?.Trim().ToLowerInvariant();
        Telefone           = telefone?.Trim();
        ContatoResponsavel = contatoResponsavel?.Trim();
        DataAtualizacao    = DateTime.UtcNow;
    }

    public void DefinirEndereco(Endereco endereco)
    {
        ArgumentNullException.ThrowIfNull(endereco);
        Endereco        = endereco;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        if (!Ativo) throw new InvalidOperationException("Fornecedor já está inativo.");
        Ativo           = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Reativar()
    {
        if (Ativo) throw new InvalidOperationException("Fornecedor já está ativo.");
        Ativo           = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public override string ToString() =>
        $"[{Id}] {NomeFantasia} ({Categoria}) — CNPJ: {CNPJ.Formatado} — {(Ativo ? "Ativo" : "Inativo")}";
}
