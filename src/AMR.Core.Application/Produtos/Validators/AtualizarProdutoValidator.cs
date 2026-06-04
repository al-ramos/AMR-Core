using FluentValidation;
using AMR.Core.Application.Produtos.Commands;

namespace AMR.Core.Application.Produtos.Validators;

public class AtualizarProdutoValidator : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PrecoUnitario).GreaterThanOrEqualTo(0).WithMessage("Preço não pode ser negativo.");
        RuleFor(x => x.EstoqueMinimo).GreaterThanOrEqualTo(0).WithMessage("Estoque mínimo não pode ser negativo.");
        RuleFor(x => x.UnidadeMedidaId).GreaterThan(0).WithMessage("UnidadeMedidaId inválido.");
    }
}
