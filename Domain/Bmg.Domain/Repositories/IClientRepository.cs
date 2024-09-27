using Bmg.Domain.Clients.Entities;

namespace Bmg.Domain.Repositories
{
    public interface IClientRepository
    {
        ValueTask CreateAsync(Client client);
        ValueTask UpdateAsync(Client client);
        ValueTask<Client?> FindByIdAsync(int id);
        ValueTask<IEnumerable<Client>> GetAllAsync();
        ValueTask DeleteAsync(Client client);
    }
}
