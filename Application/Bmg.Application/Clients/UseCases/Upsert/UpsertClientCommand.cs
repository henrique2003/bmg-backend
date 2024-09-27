using Bmg.BuildingBlocks.Database;
using Bmg.Domain.Clients.Entities;
using Bmg.Domain.Core.ValueObjects;
using Bmg.Domain.Repositories;
using CSharpFunctionalExtensions;
using MediatR;

namespace Bmg.Application.Clients.UseCases.Upsert
{
    public class UpsertClientCommand : IRequestHandler<UpsertClientRequest, Result>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpsertClientCommand(IClientRepository clientRepository, IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpsertClientRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ? await UpdateClientAsync(request, cancellationToken) : await CreateClientAsync(request, cancellationToken);
        }

        private async Task<Result> CreateClientAsync(UpsertClientRequest request, CancellationToken cancellationToken)
        {
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<Client>(emailResult.Error);
            }

            var clientResult = Client.Create(request.Name, request.Age, emailResult.Value, request.Address);
            if (clientResult.IsFailure)
            {
                return clientResult;
            }

            await _clientRepository.CreateAsync(clientResult.Value);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> UpdateClientAsync(UpsertClientRequest request, CancellationToken cancellationToken)
        {
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
            {
                return Result.Failure(emailResult.Error);
            }

            var client = await _clientRepository.FindByIdAsync(request.Id);
            if (client == null)
            {
                return Result.Failure("Cliente não encontrado");
            }

            var updateResult = client.Update(request.Name, request.Age, emailResult.Value, request.Address);
            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            await _clientRepository.UpdateAsync(client);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }

}
