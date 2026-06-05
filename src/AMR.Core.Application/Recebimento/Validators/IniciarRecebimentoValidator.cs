using FluentValidation;
using AMR.Core.Application.Recebimento.Commands;

namespace AMR.Core.Application.Recebimento.Validators;

public class IniciarRecebimentoValidator : AbstractValidator<IniciarRecebimentoCommand>
{
    public IniciarRecebimentoValidator()
    {
        RuleFor(x => x.PedidoCompraId).GreaterThan(0).WithMessage("PedidoCompraId inválido.");
    }
}
