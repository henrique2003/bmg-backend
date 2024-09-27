using Bmg.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bmg.Repository.Models.Products
{
    internal sealed class ClientMap : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("client_tb");

            builder.HasKey(c => c.Id)
                .HasName("PRIMARY");

            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            builder.Property(c => c.Age)
                .HasMaxLength(3)
                .HasColumnName("age");

            builder.OwnsOne(p => p.Email, emailBuilder => emailBuilder.Property(p => p.Value)
                .HasMaxLength(255)
                .HasColumnName("email"));

            builder.Property(c => c.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
        }
    }
}
