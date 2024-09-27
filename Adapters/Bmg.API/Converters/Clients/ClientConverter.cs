using Bmg.API.Dtos.Clients;
using Bmg.Domain.Clients.Entities;

namespace Bmg.API.Converters.Clients
{
    public static class ClientConverter
    {
        public static IEnumerable<ClientDto> Converter(IEnumerable<Client> clients)
        {
            List<ClientDto> clientDtos = [];

            foreach (var client in clients)
            {
                var clientDto = new ClientDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email.Value,
                    Address = client.Address,
                    Age = client.Age
                };

                clientDtos.Add(clientDto);
            }

            return clientDtos;
        }
    }
}
