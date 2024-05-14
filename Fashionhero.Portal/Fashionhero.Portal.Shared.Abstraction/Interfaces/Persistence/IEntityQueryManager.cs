namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence
{
    public interface IEntityQueryManager<TEntity, TSearchable> where TEntity : class, IEntity
        where TSearchable : class, ISearchable
    {
        /// <summary>
        /// Add one <typeparamref name="TEntity"/> to the database.
        /// </summary>
        /// <param name="entity">A(n) <typeparamref name="TEntity"/> to be added.</param>
        /// <returns>The newly added <typeparamref name="TEntity"/>.</returns>
        Task<TEntity> AddEntity(TEntity entity);

        /// <summary>
        /// Add multiple <typeparamref name="TEntity"/> to the database.
        /// </summary>
        /// <param name="entities">A(n) <typeparamref name="TEntity"/> to be added.</param>
        /// <returns>All the newly added <typeparamref name="TEntity"/>.</returns>
        Task<IEnumerable<TEntity>> AddEntities(IEnumerable<TEntity> entities);

        /// <summary>
        /// Get the first <typeparamref name="TEntity"/> matching properties defined in supplied <typeparamref name="TSearchable"></typeparamref>.
        /// </summary>
        /// <param name="searchable">The <typeparamref name="TSearchable"></typeparamref> with query arguments set</param>
        /// <returns>Returns first entry that matches properties defined in supplied <param name="searchable"></param></returns>
        Task<TEntity> GetEntity(TSearchable searchable);

        /// <summary>
        /// Get all <typeparamref name="TEntity"/> matching properties defined in supplied <typeparamref name="TSearchable"></typeparamref>.
        /// </summary>
        /// <param name="searchable">The <typeparamref name="TSearchable"></typeparamref> with query arguments set.</param>
        /// <returns>Returns all entries matching properties defined in supplied <param name="searchable"></param>.</returns>
        Task<IEnumerable<TEntity>> GetEntities(TSearchable searchable);

        /// <summary>
        /// Update a single <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> to be updated.</param>
        /// <returns>The updated <typeparamref name="TEntity"/>.</returns>
        Task<TEntity> UpdateEntity(TEntity entity);

        /// <summary>
        /// Update multiple <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entities">All the <typeparamref name="TEntity"/> to be updated.</param>
        /// <returns>All the updated <typeparamref name="TEntity"/>.</returns>
        Task<IEnumerable<TEntity>> UpdateEntities(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete the first <typeparamref name="TEntity"/> matching the supplied <typeparamref name="TSearchable"/>.
        /// </summary>
        /// <param name="searchable">The <typeparamref name="TSearchable"></typeparamref> with query arguments set to find what to delete</param>
        /// <returns>Nothing.</returns>
        Task DeleteEntity(TSearchable searchable);

        /// <summary>
        /// Delete the <typeparamref name="TEntity"/> with supplied <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the <typeparamref name="TEntity"/> to delete.</param>
        /// <returns>Nothing.</returns>
        Task DeleteEntityById(int id);
    }
}