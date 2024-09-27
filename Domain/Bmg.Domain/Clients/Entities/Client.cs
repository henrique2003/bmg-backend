using Bmg.Domain.Core.ValueObjects;
using CSharpFunctionalExtensions;

namespace Bmg.Domain.Clients.Entities
{
    public class Client : Entity
    {
        protected Client() { }

        private Client(string name, int age, Email email, string address)
        {
            Name = name;
            Age = age;
            Email = email;
            Address = address;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }
        public Email Email { get; private set; }
        public string Address { get; private set; }

        public static Result<Client> Create(string name, int age, Email email, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure<Client>("Nome do cliente não pode ser vazio");
            }

            if (age < 0)
            {
                return Result.Failure<Client>("Idade do cliente não pode ser negativa");
            }

            if (string.IsNullOrWhiteSpace(email.Value))
            {
                return Result.Failure<Client>("Email do cliente não pode ser vazio");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                return Result.Failure<Client>("Endereço do cliente não pode ser vazio");
            }

            var client = new Client(name, age, email, address);
            return Result.Success(client);
        }

        public Result Update(string name, int age, Email email, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure("Nome do cliente não pode ser vazio");
            }

            if (age < 0)
            {
                return Result.Failure("Idade do cliente não pode ser negativa");
            }

            if (string.IsNullOrWhiteSpace(email.Value))
            {
                return Result.Failure("Email do cliente não pode ser vazio");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                return Result.Failure("Endereço do cliente não pode ser vazio");
            }

            Name = name;
            Age = age;
            Email = email;
            Address = address;

            return Result.Success();
        }
    }

}
