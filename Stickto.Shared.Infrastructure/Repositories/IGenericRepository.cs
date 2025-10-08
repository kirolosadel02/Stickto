using Stickto.Shared.Abstractions.Entities;

namespace Stickto.Shared.Infrastructure.Repositories
{
    /// <summary>
    /// Defines a contract for a generic repository that manages entities of type <typeparamref name="TEntity"/> with identifiers of type <typeparamref name="TId"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public interface IGenericRepository<TEntity, TId> : IGenericReadRepository<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Asynchronously adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entity"/>
        /// collection is null.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Add(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously adds a range of entities to the repository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entities"/>
        /// collection is null.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously adds a range of entities to the repository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellationToken">A token that can be used to request
        /// cancellation of the operation.</param>
        /// <remarks>
        /// This method doesn't require _dbContext.SaveChanges() to commit the insertions.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entities"/>
        /// collection is null.</exception>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task BulkInsert(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entity"/>
        /// collection is null.</exception>
        void Update(TEntity entity);

        /// <summary>
        /// Updates a range of entities in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <remarks>
        /// This method updates multiple entities in a single operation. Ensure that all entities in the
        /// collection are valid and exist in the repository before calling this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entities"/>
        /// collection is null.</exception>
        public void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates a collection of entities in the repository asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        /// <remarks>
        /// This method performs a bulk update of the provided entities in the repository. It uses the provided
        /// <paramref name="cancellationToken"/> to handle task cancellation.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="entities"/> collection is null.
        /// </exception>
        Task BulkUpdate(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entity"/>
        /// collection is null.</exception>
        void Remove(TEntity entity);

        /// <summary>
        /// Removes a range of entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="entities"/>
        /// collection is null.</exception>
        void RemoveRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Removes a collection of entities from the repository asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>A task that represents the asynchronous remove operation.</returns>
        /// <remarks>
        /// This method performs a bulk removal of the provided entities from the repository. It uses the provided
        /// <paramref name="cancellationToken"/> to handle task cancellation.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="entities"/> collection is null.
        /// </exception>
        Task BulkRemove(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
        Task<TEntity> FindById(TId id, CancellationToken cancellationToken);
    }

}
