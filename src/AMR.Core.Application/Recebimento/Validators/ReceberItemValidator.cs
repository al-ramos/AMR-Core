using FluentValidation;
using AMR.Core.Application.Recebimento.Commands;

namespace AMR.Core.Application.Recebimento.Validators;

public class ReceberItemValidator : AbstractValidator<ReceberItemCommand>
{
    public ReceberItemValidator()
    {
        RuleFor(x => x.OrdemId).GreaterThan(0).WithMessage("OrdemId inválido.");
        RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("ItemId inválido.");
        RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
    }
}
