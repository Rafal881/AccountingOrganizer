using Microsoft.EntityFrameworkCore.Storage;

namespace ClientOrganizer.API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClientOrganizerDbContext _dbContext;

        public UnitOfWork(ClientOrganizerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => _dbContext.Database.BeginTransactionAsync(cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
