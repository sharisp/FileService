using Domain.SharedKernel.BaseEntity;
using Domain.SharedKernel.Interfaces;
using FileService.Infrastructure.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ICurrentUser currentUser;
        private readonly IMediator mediatR;
        private readonly BaseDbContext dbContext;
        public UnitOfWork(DbContextOptions options, ICurrentUser currentUser, IMediator mediatR, BaseDbContext dbContext)
        {
            this.currentUser = currentUser;
            this.mediatR = mediatR;
            this.dbContext = dbContext;
        }
        public int SaveChanges()
        {
            UpdateEntities(this.dbContext);
            return this.dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateEntities(this.dbContext);
            var res = await this.dbContext.SaveChangesAsync(cancellationToken);
            if (res > 0)
            {
                await DispatchEvents(this.dbContext);
            }
            return res;
        }

        public void UpdateEntities(Microsoft.EntityFrameworkCore.DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    var now = DateTimeOffset.UtcNow;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatorUserId = currentUser?.UserId;
                            entry.Entity.CreateDateTime = now;
                            entry.Entity.IsDel = false;
                            break;

                        case EntityState.Modified:
                            if (entry.HasChangedOwnedEntities() || entry.Properties.Any(p => p.IsModified))
                            {
                                entry.Entity.UpdaterUserId = currentUser?.UserId;
                                entry.Entity.UpdateDateTime = now;
                            }
                            break;

                        case EntityState.Deleted:
                            // 软删除：标记删除，不直接从数据库删除
                            entry.State = EntityState.Modified;
                            entry.Entity.IsDel = true;
                            entry.Entity.DeleterUserId = currentUser?.UserId;
                            entry.Entity.DeleteDateTime = now;
                            break;
                    }
                }
            }
        }

        public async Task DispatchEvents(Microsoft.EntityFrameworkCore.DbContext? context)
        {
            if (context == null) return;

            var entities = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.GetDomainEvents().Any())
                .Select(e => e.Entity);

            var domainEvents = entities
                .SelectMany(e => e.GetDomainEvents())
                .ToList();

            entities.ToList().ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediatR.Publish(domainEvent);

        }

    }
}
