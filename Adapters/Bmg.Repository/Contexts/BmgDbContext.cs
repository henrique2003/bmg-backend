using Bmg.Domain.Clients.Entities;
using Bmg.Repository.Models.Products;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bmg.Repository.Contexts
{
    public partial class BmgDbContext : DbContext, IDataProtectionKeyContext
    {
        public BmgDbContext()
        {
        }

        public BmgDbContext(DbContextOptions<BmgDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientMap).Assembly);
        }
    }
}
