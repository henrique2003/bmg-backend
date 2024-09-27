using CSharpFunctionalExtensions;
using MediatR;

namespace Bmg.Application.Clients.UseCases.Delete
{
    public class DeleteClientRequest : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
