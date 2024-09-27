using FluentValidation;

namespace Bmg.Application.Clients.UseCases.Delete
{
    public class DeleteClientValidator : AbstractValidator<DeleteClientRequest>
    {
        public DeleteClientValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty()
                .WithMessage("Id não pode ser vazio")
                .GreaterThan(0)
                .WithMessage("Id dever ser menor que 0");
        }
    }
}
