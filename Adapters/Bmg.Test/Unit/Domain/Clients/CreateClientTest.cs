using Bmg.Domain.Clients.Entities;
using Bmg.Domain.Core.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Bmg.Test.Unit.Domain.Clients
{
    public class CreateClientTest
    {
        [Fact]
        public void Create_ShouldReturnFailure_WhenNameIsNull()
        {
            string name = null;
            int age = 25;
            Email email = Email.Create("validemail@example.com").Value;
            string address = "Valid address";

            var result = Client.Create(name, age, email, address);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Nome do cliente não pode ser vazio");
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenAgeIsNegative()
        {
            string name = "Valid Name";
            int age = -1;
            Email email = Email.Create("validemail@example.com").Value;
            string address = "Valid address";

            var result = Client.Create(name, age, email, address);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Idade do cliente não pode ser negativa");
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenEmailIsNull()
        {
            string name = "Valid Name";
            int age = 25;
            Email email = Email.CreateForTest().Value;
            string address = "Valid address";

            var result = Client.Create(name, age, email, address);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Email do cliente não pode ser vazio");
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenAddressIsNull()
        {
            string name = "Valid Name";
            int age = 25;
            Email email = Email.Create("validemail@example.com").Value;
            string address = null;

            var result = Client.Create(name, age, email, address);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Endereço do cliente não pode ser vazio");
        }

        [Fact]
        public void Create_ShouldReturnSuccess_WhenValidParametersAreProvided()
        {
            string name = "Valid Name";
            int age = 25;
            Email email = Email.Create("validemail@example.com").Value;
            string address = "Valid address";

            var result = Client.Create(name, age, email, address);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
            result.Value.Age.Should().Be(age);
            result.Value.Email.Should().Be(email);
            result.Value.Address.Should().Be(address);
        }
    }

}
