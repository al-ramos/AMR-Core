namespace AMR.Core.Domain.Entities;

public class UnidadeMedida
{
    public int    Id        { get; private set; }
    public string Sigla     { get; private set; }
    public string Descricao { get; private set; }
    public bool   Ativo     { get; private set; }

    protected UnidadeMedida() { Sigla = null!; Descricao = null!; }

    private UnidadeMedida(string sigla, string descricao)
    {
        if (string.IsNullOrWhiteSpace(sigla))
            throw new ArgumentException("Sigla é obrigatória.", nameof(sigla));
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(descricao));

        Sigla     = sigla.Trim().ToUpper();
        Descricao = descricao.Trim();
        Ativo     = true;
    }

    public static UnidadeMedida Criar(string sigla, string descricao) => new(sigla, descricao);

    public void Inativar() => Ativo = false;
    public void Reativar() => Ativo = true;

    public override string ToString() => $"{Sigla} — {Descricao}";
}
