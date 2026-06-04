using FluentValidation;
using AMR.Core.Application.PedidosCompra.Commands;

namespace AMR.Core.Application.PedidosCompra.Validators;

public class CriarPedidoCompraValidator : AbstractValidator<CriarPedidoCompraCommand>
{
    public CriarPedidoCompraValidator()
    {
        RuleFor(x => x.EmpresaId).GreaterThan(0).WithMessage("EmpresaId inválido.");
        RuleFor(x => x.FornecedorId).GreaterThan(0).WithMessage("FornecedorId inválido.");
        RuleFor(x => x.Itens).NotEmpty().WithMessage("Pedido deve ter ao menos um item.");
        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.ProdutoId).GreaterThan(0).WithMessage("ProdutoId inválido.");
            item.RuleFor(i => i.Quantidade).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
            item.RuleFor(i => i.PrecoUnitario).GreaterThanOrEqualTo(0).WithMessage("Preço não pode ser negativo.");
        });
    }
}
