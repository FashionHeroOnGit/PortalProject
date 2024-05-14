using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.DataAccess.Core
{
    public abstract class
        BaseEntityQueryManager<TContext, TEntity, TSearchable> : IEntityQueryManager<TEntity, TSearchable>
        where TContext : BaseDatabaseContext where TEntity : class, IEntity where TSearchable : class, ISearchable
    {
        protected readonly TContext context;

        protected BaseEntityQueryManager(TContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public Task<TEntity> AddEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> AddEntities(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<TEntity> GetEntity(TSearchable searchable)
        {
            return BuildQuery(searchable).ToList().First();
        }

        private IQueryable<TEntity> BuildQuery(TSearchable searchable)
        {
            var query = GetBaseQuery();

            if (searchable.Id != default)
                query = query.Where(x => x.Id == searchable.Id);

            query = AddQueryArguments(searchable, query);

            return query;
        }

        protected abstract IQueryable<TEntity> GetBaseQuery();
        protected abstract IQueryable<TEntity> AddQueryArguments(TSearchable searchable, IQueryable<TEntity> query);

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> GetEntities(TSearchable searchable)
        {
            return BuildQuery(searchable).ToList();
        }

        /// <inheritdoc />
        public async Task<TEntity> UpdateEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> UpdateEntities(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task DeleteEntity(TSearchable searchable)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task DeleteEntityById(int id)
        {
            throw new NotImplementedException();
        }
    }
}