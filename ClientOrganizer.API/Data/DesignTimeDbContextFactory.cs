using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClientOrganizer.API.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ClientOrganizerDbContext>
    {
        public ClientOrganizerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClientOrganizerDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=ClientOrganizer;Trusted_Connection=True;MultipleActiveResultSets=true";

            var sqlPassword = Environment.GetEnvironmentVariable("sqlPassword");
            if (!string.IsNullOrWhiteSpace(sqlPassword) && !connectionString.Contains("Password="))
            {
                connectionString += $";Password={sqlPassword}";
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new ClientOrganizerDbContext(optionsBuilder.Options);
        }
    }
}
