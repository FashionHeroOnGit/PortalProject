using System.Diagnostics.CodeAnalysis;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fashionhero.Portal.DataAccess.Core
{
    public abstract class BaseDatabaseContext : DbContext
    {
        private readonly DbContextOptions options;

        protected BaseDatabaseContext([NotNull] DbContextOptions options) : base(options)
        {
            this.options = options;
        }

        public override int SaveChanges()
        {
            UpdateDatetimes();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateDatetimes();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateDatetimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateDatetimes();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateDatetimes()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e is {Entity: IEntity, State: EntityState.Added or EntityState.Modified,}).ToList();

            foreach (EntityEntry entityEntry in entries)
            {
                ((IEntity) entityEntry.Entity).UpdatedDateTime = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                    ((IEntity) entityEntry.Entity).CreatedDateTime = DateTime.Now;
            }
        }
    }
}