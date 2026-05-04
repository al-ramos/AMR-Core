using AMR.Core.Domain.ValueObjects;

namespace AMR.Core.Domain.Entities;

/// <summary>
/// Entidade Cliente do domínio ERP.
/// Pode ser Pessoa Jurídica (CNPJ) ou Pessoa Física (CPF).
/// </summary>
public class Cliente
{
    public int      Id           { get; private set; }
    public string   Nome         { get; private set; }
    public string   TipoDocumento { get; private set; }  // "CNPJ" ou "CPF"
    public string   NumeroDocumento { get; private set; } // apenas dígitos
    public CNPJ?    CNPJ         { get; private set; }   // preenchido se PJ
    public string?  Email        { get; private set; }
    public string?  Telefone     { get; private set; }
    public Endereco? Endereco    { get; private set; }
    public bool     Ativo        { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Vínculo com a empresa dona deste cadastro
    public int     EmpresaId  { get; private set; }
    public Empresa? Empresa   { get; private set; }

    // EF Core
    protected Cliente() { Nome = null!; TipoDocumento = null!; NumeroDocumento = null!; }

    private Cliente(
        int empresaId, string nome, string tipoDocumento, string numeroDocumento,
        CNPJ? cnpj, string? email, string? telefone, Endereco? endereco)
    {
        if (empresaId <= 0)
            throw new ArgumentException("EmpresaId inválido.", nameof(empresaId));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));

        if (tipoDocumento != "CNPJ" && tipoDocumento != "CPF")
            throw new ArgumentException("TipoDocumento deve ser 'CNPJ' ou 'CPF'.", nameof(tipoDocumento));

        if (string.IsNullOrWhiteSpace(numeroDocumento))
            throw new ArgumentException("Número do documento é obrigatório.", nameof(numeroDocumento));

        EmpresaId      = empresaId;
        Nome           = nome.Trim();
        TipoDocumento  = tipoDocumento;
        NumeroDocumento = new string(numeroDocumento.Where(char.IsDigit).ToArray());
        CNPJ           = cnpj;
        Email          = email?.Trim().ToLowerInvariant();
        Telefone       = telefone?.Trim();
        Endereco       = endereco;
        Ativo          = true;
        DataCadastro   = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria um cliente Pessoa Jurídica.
    /// </summary>
    public static Cliente CriarPJ(
        int empresaId, string nome, CNPJ cnpj,
        string? email = null, string? telefone = null, Endereco? endereco = null)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        return new Cliente(empresaId, nome, "CNPJ", cnpj.Numero, cnpj, email, telefone, endereco);
    }

    /// <summary>
    /// Cria um cliente Pessoa Física.
    /// </summary>
    public static Cliente CriarPF(
        int empresaId, string nome, string cpf,
        string? email = null, string? telefone = null, Endereco? endereco = null)
    {
        var cpfDigitos = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpfDigitos.Length != 11)
            throw new ArgumentException($"CPF deve conter 11 dígitos: '{cpf}'.", nameof(cpf));

        return new Cliente(empresaId, nome, "CPF", cpfDigitos, null, email, telefone, endereco);
    }

    // ── Comportamentos ─────────────────────────────────────────────────────────

    public void AtualizarContato(string? email, string? telefone)
    {
        Email           = email?.Trim().ToLowerInvariant();
        Telefone        = telefone?.Trim();
        DataAtualizacao = DateTime.UtcNow;
    }

    public void DefinirEndereco(Endereco endereco)
    {
        ArgumentNullException.ThrowIfNull(endereco);
        Endereco        = endereco;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        if (!Ativo) throw new InvalidOperationException("Cliente já está inativo.");
        Ativo           = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Reativar()
    {
        if (Ativo) throw new InvalidOperationException("Cliente já está ativo.");
        Ativo           = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool EhPessoaJuridica => TipoDocumento == "CNPJ";
    public bool EhPessoaFisica   => TipoDocumento == "CPF";

    public override string ToString() =>
        $"[{Id}] {Nome} — {TipoDocumento}: {NumeroDocumento} — {(Ativo ? "Ativo" : "Inativo")}";
}
