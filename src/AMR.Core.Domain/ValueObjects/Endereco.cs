namespace AMR.Core.Domain.ValueObjects;

public sealed class Endereco : IEquatable<Endereco>
{
    public string  Logradouro   { get; private set; } = null!;
    public string  Numero       { get; private set; } = null!;
    public string? Complemento  { get; private set; }
    public string  Bairro       { get; private set; } = null!;
    public string  Cidade       { get; private set; } = null!;
    public string  Estado       { get; private set; } = null!;
    public string  CEP          { get; private set; } = null!;
    public string  Pais         { get; private set; } = null!;

    public string CEPFormatado => $"{CEP[..5]}-{CEP[5..]}";

    // EF Core
    private Endereco() { }

    private Endereco(
        string logradouro, string numero, string? complemento,
        string bairro, string cidade, string estado, string cep, string pais)
    {
        Logradouro  = logradouro;
        Numero      = numero;
        Complemento = complemento;
        Bairro      = bairro;
        Cidade      = cidade;
        Estado      = estado;
        CEP         = cep;
        Pais        = pais;
    }

    public static Endereco Criar(
        string logradouro, string numero, string? complemento,
        string bairro, string cidade, string estado, string cep,
        string pais = "Brasil")
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro é obrigatório.", nameof(logradouro));
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número é obrigatório.", nameof(numero));
        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro é obrigatório.", nameof(bairro));
        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade é obrigatória.", nameof(cidade));

        var ufNorm = (estado ?? "").Trim().ToUpper();
        if (ufNorm.Length != 2 || !UFsValidas.Contains(ufNorm))
            throw new ArgumentException($"Estado (UF) inválido: '{estado}'.", nameof(estado));

        var cepDigitos = new string((cep ?? "").Where(char.IsDigit).ToArray());
        if (cepDigitos.Length != 8)
            throw new ArgumentException($"CEP deve conter 8 dígitos: '{cep}'.", nameof(cep));

        return new Endereco(
            logradouro.Trim(), numero.Trim(), complemento?.Trim(),
            bairro.Trim(), cidade.Trim(), ufNorm, cepDigitos, pais.Trim());
    }

    private static readonly HashSet<string> UFsValidas =
    [
        "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA",
        "MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN",
        "RS","RO","RR","SC","SP","SE","TO"
    ];

    public bool Equals(Endereco? other) =>
        other is not null &&
        Logradouro == other.Logradouro && Numero == other.Numero &&
        Complemento == other.Complemento && Bairro == other.Bairro &&
        Cidade == other.Cidade && Estado == other.Estado &&
        CEP == other.CEP && Pais == other.Pais;

    public override bool Equals(object? obj) => obj is Endereco other && Equals(other);
    public override int GetHashCode() =>
        HashCode.Combine(Logradouro, Numero, Bairro, Cidade, Estado, CEP);
    public override string ToString() =>
        $"{Logradouro}, {Numero}{(string.IsNullOrEmpty(Complemento) ? "" : $" {Complemento}")} — {Bairro}, {Cidade}/{Estado} — {CEPFormatado}";

    public static bool operator ==(Endereco? a, Endereco? b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(Endereco? a, Endereco? b) => !(a == b);
}
