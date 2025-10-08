using Stickto.Shared.Abstractions.Entities;
using Stickto.Shared.Infrastructure.Repositories;

namespace Stickto.Shared.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Defines a contract for a unit of work, which coordinates the work of multiple repositories by creating a single database context class shared among all the repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Retrieves a repository for managing entities of type <typeparamref name="TEntity"/> with identifiers of type <typeparamref name="TId"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
        /// <returns>The repository.</returns>
        IGenericRepository<TEntity, TId> Repository<TEntity, TId>()
            where TEntity : Entity;

        IGenericReadRepository<TEntity> ViewRepository<TEntity>()
            where TEntity : Entity;

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction asynchronously.
        /// If an error occurs during the commit, the transaction is rolled back.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets all repositories managed by this unit of work.
        /// </summary>
        void ResetRepositories();
    }
}
