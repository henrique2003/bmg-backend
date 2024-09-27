using Bmg.Domain.Clients.Entities;
using Bmg.Domain.Repositories;
using Bmg.Repository.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bmg.Repository.Models.Products
{
    public class ClientRepository : IClientRepository
    {
        private readonly BmgDbContext _bmgDbContext;

        public ClientRepository(BmgDbContext bmgDbContext)
        {
            _bmgDbContext = bmgDbContext;
        }

        public async ValueTask CreateAsync(Client client)
        {
            await _bmgDbContext.Clients.AddAsync(client);
        }

        public async ValueTask DeleteAsync(Client client)
        {
            _bmgDbContext.Clients.Remove(client);

            await Task.CompletedTask;
        }

        public async ValueTask<Client?> FindByIdAsync(int id)
        {
            return await _bmgDbContext.Clients
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async ValueTask<IEnumerable<Client>> GetAllAsync()
        {
            return await _bmgDbContext.Clients.ToListAsync();
        }

        public async ValueTask UpdateAsync(Client client)
        {
            _bmgDbContext.Clients.Update(client);

            await Task.CompletedTask;
        }
    }
}
