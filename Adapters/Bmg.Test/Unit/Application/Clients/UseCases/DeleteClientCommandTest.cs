using Bmg.Application.Clients.UseCases.Delete;
using Bmg.BuildingBlocks.Database;
using Bmg.Domain.Clients.Entities;
using Bmg.Domain.Core.ValueObjects;
using Bmg.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bmg.Test.Unit.Application.Clients.UseCases
{
    public class DeleteClientCommandTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteClientCommand _deleteClientCommand;

        public DeleteClientCommandTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _deleteClientCommand = new DeleteClientCommand(_clientRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        #region Handle Tests

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenClientNotFound()
        {
            var request = new DeleteClientRequest { Id = 1 };
            var cancellationToken = CancellationToken.None;

            _clientRepositoryMock
                .Setup(repo => repo.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Client)null);

            var result = await _deleteClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Cliente não encontrado");
            _clientRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Client>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryAndUnitOfWork_WhenClientFound()
        {
            var client = Client.Create("John Doe", 30, Email.Create("john@example.com").Value, "Address 123").Value;
            var request = new DeleteClientRequest { Id = (int)client.Id };
            var cancellationToken = CancellationToken.None;

            _clientRepositoryMock
                .Setup(repo => repo.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(client);

            var result = await _deleteClientCommand.Handle(request, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            _clientRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Client>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Once);
        }

        #endregion
    }

}
