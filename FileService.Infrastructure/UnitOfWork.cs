using FileService.Domain;
using FileService.Domain.Interface;
using FileService.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BaseDbContext dbContext;
        public UnitOfWork(DbContextOptions options, BaseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            return await dbContext.SaveChangesAsync(cancellationToken);

        }

    }
}
