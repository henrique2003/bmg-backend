using Bmg.BuildingBlocks.Database;
using Bmg.Domain.Repositories;
using CSharpFunctionalExtensions;
using MediatR;

namespace Bmg.Application.Clients.UseCases.Delete
{
    public class DeleteClientCommand : IRequestHandler<DeleteClientRequest, Result>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteClientCommand(IClientRepository clientRepository, IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteClientRequest request, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.FindByIdAsync(request.Id);
            if (client == null)
            {
                return Result.Failure("Cliente não encontrado");
            }

            await _clientRepository.DeleteAsync(client);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success(client);
        }
    }

}
