using Microsoft.EntityFrameworkCore;
using ClientOrganizer.API.Models;

namespace ClientOrganizer.API.Data
{
    public class ClientOrganizerDbContext : DbContext
    {
        public ClientOrganizerDbContext(DbContextOptions<ClientOrganizerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<FinancialData> FinancialData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.NipNb)
                .IsUnique();

            modelBuilder.Entity<FinancialData>()
                .HasIndex(f => new { f.ClientId, f.Month, f.Year })
                .IsUnique();

            modelBuilder.Entity<FinancialData>()
                .HasOne(f => f.Client)
                .WithMany(c => c.FinancialRecords)
                .HasForeignKey(f => f.ClientId);
        }
    }
}
