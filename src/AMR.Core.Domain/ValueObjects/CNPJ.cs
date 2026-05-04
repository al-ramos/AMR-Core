namespace AMR.Core.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um CNPJ válido.
/// Armazena apenas os 14 dígitos numéricos e valida os dígitos verificadores.
/// </summary>
public sealed class CNPJ : IEquatable<CNPJ>
{
    private readonly string _valor;

    public string Numero => _valor;
    public string Formatado => $"{_valor[..2]}.{_valor[2..5]}.{_valor[5..8]}/{_valor[8..12]}-{_valor[12..]}";

    private CNPJ(string numero) => _valor = numero;

    /// <summary>
    /// Cria um novo CNPJ a partir de uma string, aceitando formatado ou apenas dígitos.
    /// Lança ArgumentException se inválido.
    /// </summary>
    public static CNPJ Criar(string cnpj)
    {
        var digitos = new string(cnpj.Where(char.IsDigit).ToArray());

        if (digitos.Length != 14)
            throw new ArgumentException($"CNPJ deve conter 14 dígitos. Recebido: '{cnpj}'.", nameof(cnpj));

        if (TodosDigitosIguais(digitos))
            throw new ArgumentException($"CNPJ inválido (todos dígitos iguais): '{cnpj}'.", nameof(cnpj));

        if (!ValidarDigitosVerificadores(digitos))
            throw new ArgumentException($"CNPJ com dígitos verificadores inválidos: '{cnpj}'.", nameof(cnpj));

        return new CNPJ(digitos);
    }

    /// <summary>
    /// Tenta criar um CNPJ sem lançar exceção.
    /// </summary>
    public static bool TentarCriar(string cnpj, out CNPJ? resultado)
    {
        try
        {
            resultado = Criar(cnpj);
            return true;
        }
        catch
        {
            resultado = null;
            return false;
        }
    }

    private static bool TodosDigitosIguais(string digitos) =>
        digitos.Distinct().Count() == 1;

    private static bool ValidarDigitosVerificadores(string cnpj)
    {
        int[] multiplicadores1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplicadores2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var digito1 = CalcularDigito(cnpj[..12], multiplicadores1);
        var digito2 = CalcularDigito(cnpj[..13], multiplicadores2);

        return cnpj[12] == digito1 && cnpj[13] == digito2;
    }

    private static char CalcularDigito(string base_, int[] multiplicadores)
    {
        var soma = base_
            .Select((c, i) => (c - '0') * multiplicadores[i])
            .Sum();

        var resto = soma % 11;
        return resto < 2 ? '0' : (char)('0' + (11 - resto));
    }

    public bool Equals(CNPJ? other) => other is not null && _valor == other._valor;
    public override bool Equals(object? obj) => obj is CNPJ other && Equals(other);
    public override int GetHashCode() => _valor.GetHashCode();
    public override string ToString() => Formatado;

    public static bool operator ==(CNPJ? a, CNPJ? b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(CNPJ? a, CNPJ? b) => !(a == b);
}
