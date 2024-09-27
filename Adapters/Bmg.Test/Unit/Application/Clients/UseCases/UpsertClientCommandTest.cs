using Bmg.Application.Clients.UseCases.Upsert;
using Bmg.BuildingBlocks.Database;
using Bmg.Domain.Clients.Entities;
using Bmg.Domain.Core.ValueObjects;
using Bmg.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bmg.Test.Unit.Application.Clients.UseCases
{
    public class UpsertClientCommandTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpsertClientCommand _upsertClientCommand;

        public UpsertClientCommandTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _upsertClientCommand = new UpsertClientCommand(_clientRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        #region Handle Tests

        [Fact]
        public async Task Handle_ShouldCallUpdate_WhenRequestIdIsGreaterThanZero()
        {
            var request = new UpsertClientRequest { Id = 1, Name = "Client 1", Age = 30, Email = "client1@example.com", Address = "Address 1" };
            var cancellationToken = CancellationToken.None;

            _clientRepositoryMock
                .Setup(repo => repo.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Client.Create("Client 1", 30, Email.Create("client1@example.com").Value, "Address 1").Value);

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            _clientRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallCreate_WhenRequestIdIsZero()
        {
            var request = new UpsertClientRequest { Id = 0, Name = "Client 1", Age = 30, Email = "client1@example.com", Address = "Address 1" };
            var cancellationToken = CancellationToken.None;

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            _clientRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Client>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Once);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ShouldReturnFailure_WhenClientCreationFails()
        {
            var request = new UpsertClientRequest { Name = null, Age = 30, Email = "client1@example.com", Address = "Address 1" };
            var cancellationToken = CancellationToken.None;

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeFalse();
            _clientRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Client>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task Create_ShouldCallRepositoryAndUnitOfWork_WhenClientCreationSucceeds()
        {
            var request = new UpsertClientRequest { Name = "New Client", Age = 25, Email = "newclient@example.com", Address = "New Address" };
            var cancellationToken = CancellationToken.None;

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            _clientRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Client>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenClientNotFound()
        {
            var request = new UpsertClientRequest { Id = 1, Name = "Updated Client", Age = 40, Email = "updatedclient@example.com", Address = "Updated Address" };
            var cancellationToken = CancellationToken.None;

            _clientRepositoryMock.Setup(repo => repo.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((Client)null);

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Cliente não encontrado");
            _clientRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenUpdateFails()
        {
            var request = new UpsertClientRequest { Id = 1, Name = null, Age = 40, Email = "updatedclient@example.com", Address = "Updated Address" };
            var client = Client.Create("Existing Client", 30, Email.Create("existingclient@example.com").Value, "Existing Address").Value;
            var cancellationToken = CancellationToken.None;

            _clientRepositoryMock.Setup(repo => repo.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(client);

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeFalse();
            _clientRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldCallRepositoryAndUnitOfWork_WhenUpdateSucceeds()
        {
            var request = new UpsertClientRequest { Id = 1, Name = "Updated Client", Age = 40, Email = "updatedclient@example.com", Address = "Updated Address" };
            var client = Client.Create("Existing Client", 30, Email.Create("existingclient@example.com").Value, "Existing Address").Value;
            var cancellationToken = CancellationToken.None;

            _clientRepositoryMock.Setup(repo => repo.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(client);

            var result = await _upsertClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            _clientRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Once);
        }

        #endregion
    }

}
