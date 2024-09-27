using FluentValidation;

namespace Bmg.Application.Clients.UseCases.Upsert
{
    public class UpsertClientValidator : AbstractValidator<UpsertClientRequest>
    {
        public UpsertClientValidator()
        {
            RuleFor(client => client.Name)
                .NotEmpty()
                .WithMessage("Nome não pode ser vazio");

            RuleFor(client => client.Age)
                .GreaterThan(0)
                .WithMessage("Idade deve ser maior que 0");

            RuleFor(client => client.Email)
                .NotEmpty()
                .WithMessage("Email não pode ser vazio")
                .EmailAddress()
                .WithMessage("Email deve ser válido");

            RuleFor(client => client.Address)
                .NotEmpty()
                .WithMessage("Endereço não pode ser vazio");
        }
    }
}
