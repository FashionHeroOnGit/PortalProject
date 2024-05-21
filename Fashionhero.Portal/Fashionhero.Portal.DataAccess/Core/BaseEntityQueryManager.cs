using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.DataAccess.Core
{
    public abstract class
        BaseEntityQueryManager<TContext, TEntity, TSearchable> : IEntityQueryManager<TEntity, TSearchable>
        where TContext : BaseDatabaseContext
        where TEntity : class, IEntity
        where TSearchable : class, ISearchable, new()
    {
        protected readonly TContext context;
        protected readonly ILogger logger;

        protected BaseEntityQueryManager(TContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<TEntity> AddEntity(TEntity entity)
        {
            await context.AddAsync(entity);
            await SaveChanges();

            return await GetEntity(new TSearchable() {CreatedDateTime = DateTime.Now,});
        }

        public async Task<IEnumerable<TEntity>> AddEntities(IEnumerable<TEntity> entities)
        {
            var enumerable = entities.ToList();
            try
            {
                context.AddRange(enumerable);
                await SaveChanges();

                return await GetEntities(new TSearchable() {CreatedDateTime = DateTime.Now,});
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    $"Exception thrown while attempting to add {enumerable.Count} entities ({typeof(TEntity).Name}).");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<TEntity> GetEntity(TSearchable searchable)
        {
            return (await BuildQuery(searchable).ToListAsync()).First();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> GetEntities(TSearchable searchable)
        {
            return await BuildQuery(searchable).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<TEntity> UpdateEntity(TEntity entity)
        {
            context.Update(entity);
            await SaveChanges();

            return await GetEntity(new TSearchable() {UpdatedDateTime = DateTime.Now,});
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> UpdateEntities(IEnumerable<TEntity> entities)
        {
            var enumerable = entities.ToList();
            try
            {
                context.UpdateRange(enumerable);
                await SaveChanges();

                return await GetEntities(new TSearchable() {UpdatedDateTime = DateTime.Now,});
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    $"Exception thrown while attempting to update {enumerable.Count} entities ({typeof(TEntity).Name}).");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteEntity(TSearchable searchable)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task DeleteEntities(IEnumerable<TEntity> entities)
        {
            var enumerable = entities.ToList();
            try
            {
                context.RemoveRange(enumerable);
                await SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    $"Exception thrown while attempting to delete {enumerable.Count} entities ({typeof(TEntity).Name}).");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteEntityById(int id)
        {
            throw new NotImplementedException();
        }

        private async Task SaveChanges()
        {
            int entriesChanged = await context.SaveChangesAsync();
            logger.LogInformation($"{entriesChanged} Changed object(s) saved to Database");
        }

        private IQueryable<TEntity> BuildQuery(TSearchable searchable)
        {
            var query = GetBaseQuery();

            if (searchable.Id != default)
                query = query.Where(x => x.Id == searchable.Id);
            if (searchable.CreatedDateTime != default)
            {
                DateTime compareDate = searchable.CreatedDateTime.AddMinutes(-5);
                query = query.Where(x => x.CreatedDateTime >= compareDate);
            }

            if (searchable.UpdatedDateTime != default)
            {
                DateTime compareDate = searchable.UpdatedDateTime.AddMinutes(-5);
                query = query.Where(x => x.UpdatedDateTime >= compareDate);
            }

            query = AddQueryArguments(searchable, query);

            return query;
        }

        protected abstract IQueryable<TEntity> GetBaseQuery();
        protected abstract IQueryable<TEntity> AddQueryArguments(TSearchable searchable, IQueryable<TEntity> query);
    }
}