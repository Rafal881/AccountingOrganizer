using Microsoft.EntityFrameworkCore;
using ClientOrganizer.API.Models.Entities;

namespace ClientOrganizer.API.Data
{
    public class ClientOrganizerDbContext : DbContext
    {
        public ClientOrganizerDbContext(DbContextOptions<ClientOrganizerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<FinancialData> FinancialData => Set<FinancialData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientOrganizerDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
