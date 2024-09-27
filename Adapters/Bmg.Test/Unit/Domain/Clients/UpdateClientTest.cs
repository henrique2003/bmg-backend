using Bmg.Domain.Clients.Entities;
using Bmg.Domain.Core.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Bmg.Test.Unit.Domain.Products
{
    public class UpdateClientTest
    {
        [Fact]
        public void Update_ShouldReturnFailure_WhenNameIsNull()
        {
            var client = Client.Create("Initial Name", 25, Email.Create("initial@example.com").Value, "Initial Address").Value;
            string newName = null;
            int newAge = 30;
            Email newEmail = Email.Create("new@example.com").Value;
            string newAddress = "New Address";

            var result = client.Update(newName, newAge, newEmail, newAddress);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Nome do cliente não pode ser vazio");
        }

        [Fact]
        public void Update_ShouldReturnFailure_WhenAgeIsNegative()
        {
            var client = Client.Create("Initial Name", 25, Email.Create("initial@example.com").Value, "Initial Address").Value;
            string newName = "Updated Name";
            int newAge = -1;
            Email newEmail = Email.Create("new@example.com").Value;
            string newAddress = "New Address";

            var result = client.Update(newName, newAge, newEmail, newAddress);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Idade do cliente não pode ser negativa");
        }

        [Fact]
        public void Update_ShouldReturnFailure_WhenEmailIsNull()
        {
            var client = Client.Create("Initial Name", 25, Email.Create("initial@example.com").Value, "Initial Address").Value;
            string newName = "Updated Name";
            int newAge = 30;
            Email newEmail = Email.CreateForTest().Value;
            string newAddress = "New Address";

            var result = client.Update(newName, newAge, newEmail, newAddress);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Email do cliente não pode ser vazio");
        }

        [Fact]
        public void Update_ShouldReturnFailure_WhenAddressIsNull()
        {
            var client = Client.Create("Initial Name", 25, Email.Create("initial@example.com").Value, "Initial Address").Value;
            string newName = "Updated Name";
            int newAge = 30;
            Email newEmail = Email.Create("new@example.com").Value;
            string newAddress = null;

            var result = client.Update(newName, newAge, newEmail, newAddress);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Endereço do cliente não pode ser vazio");
        }

        [Fact]
        public void Update_ShouldReturnSuccess_WhenValidParametersAreProvided()
        {
            var client = Client.Create("Initial Name", 25, Email.Create("initial@example.com").Value, "Initial Address").Value;
            string newName = "Updated Name";
            int newAge = 30;
            Email newEmail = Email.Create("new@example.com").Value;
            string newAddress = "Updated Address";

            var result = client.Update(newName, newAge, newEmail, newAddress);

            result.IsSuccess.Should().BeTrue();
            client.Name.Should().Be(newName);
            client.Age.Should().Be(newAge);
            client.Email.Should().Be(newEmail);
            client.Address.Should().Be(newAddress);
        }
    }
}
