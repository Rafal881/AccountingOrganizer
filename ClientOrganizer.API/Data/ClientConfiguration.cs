using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientOrganizer.API.Data
{
    public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> client)
        {
            client.HasKey(c => c.Id);            

            client.Property(c => c.Name).IsRequired().HasMaxLength(200);
            client.Property(c => c.Address).IsRequired().HasMaxLength(500);
            client.Property(c => c.NipNb).IsRequired().HasMaxLength(20);
            client.Property(c => c.Email).IsRequired().HasMaxLength(256);

            client.HasIndex(c => c.NipNb).IsUnique();
        }
    }
}
