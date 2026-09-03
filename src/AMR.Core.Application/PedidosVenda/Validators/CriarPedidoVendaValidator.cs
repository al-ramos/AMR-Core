using FluentValidation;
using AMR.Core.Application.PedidosVenda.Commands;

namespace AMR.Core.Application.PedidosVenda.Validators;

public class CriarPedidoVendaValidator : AbstractValidator<CriarPedidoVendaCommand>
{
    public CriarPedidoVendaValidator()
    {
        RuleFor(x => x.EmpresaId).GreaterThan(0).WithMessage("EmpresaId inválido.");
        RuleFor(x => x.ClienteId).GreaterThan(0).WithMessage("ClienteId inválido.");
        RuleFor(x => x.Itens).NotEmpty().WithMessage("Pedido deve ter ao menos um item.");
        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.ProdutoId).GreaterThan(0).WithMessage("ProdutoId inválido.");
            item.RuleFor(i => i.Quantidade).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
            item.RuleFor(i => i.PrecoUnitario).GreaterThanOrEqualTo(0).WithMessage("Preço não pode ser negativo.");
            item.RuleFor(i => i.Desconto).InclusiveBetween(0, 100).WithMessage("Desconto deve estar entre 0 e 100.");
        });
    }
}
