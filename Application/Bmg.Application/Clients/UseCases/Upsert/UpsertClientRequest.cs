using CSharpFunctionalExtensions;
using MediatR;

namespace Bmg.Application.Clients.UseCases.Upsert
{
    public class UpsertClientRequest : IRequest<Result>
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
