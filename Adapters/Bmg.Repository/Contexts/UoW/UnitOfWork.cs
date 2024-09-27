using Bmg.BuildingBlocks.Database;

namespace Bmg.Repository.Contexts.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BmgDbContext _bmgDbContext;

        public UnitOfWork(BmgDbContext bmgDbContext)
        {
            _bmgDbContext = bmgDbContext;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _bmgDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
