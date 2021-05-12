using Medical.Service.Factory;
using Medical.Entities.DomainEntity;
using Medical.Interface.DbContext;
using Medical.Interface.Repository;
using Medical.Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medical.Service
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected ICoreDbContext context;
        public UnitOfWork(ICoreDbContext context)
        {
            this.context = context;
            if (this.context != null)
            {
                this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }

        public UnitOfWork(IDbContextFactory dbContextFactory)
        {
            context = dbContextFactory.Create();
            if (context != null)
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }
        public abstract ICatalogueRepository<T> CatalogueRepository<T>() where T : MedicalCatalogueAppDomain;

        public abstract IDomainRepository<T> Repository<T>() where T : MedicalAppDomain;

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return context.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
