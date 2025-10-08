using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Stickto.Shared.Abstractions.Entities;
using Stickto.Shared.Infrastructure.Repositories;
using System.Collections.Concurrent;

namespace Stickto.Shared.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Provides a unit of work for managing entities,
    /// coordinating the work of multiple repositories
    /// by creating a single database context class shared among all the repositories.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories = new();
        private readonly ConcurrentDictionary<string, object> _viewRepositories = new();
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class with the specified context and HTTP context accessor.
        /// </summary>
        /// <param name="context">The context to use for the unit of work.</param>
        /// <param name="serviceProvider">The service provider for app.</param>
        public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            this._context = context;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Retrieves a repository for managing entities of type <typeparamref name="TEntity"/> with identifiers of type <typeparamref name="TId"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
        /// <returns>The repository.</returns>
        public IGenericRepository<TEntity, TId> Repository<TEntity, TId>()
            where TEntity : Entity
        {
            var key = $"{typeof(TEntity).Name}_{typeof(TId).Name}";

            return (IGenericRepository<TEntity, TId>)_repositories.GetOrAdd(key, _ =>
            {
                return ActivatorUtilities.CreateInstance<GenericRepository<TEntity, TId>>(_serviceProvider, _context);
            });
        }

        /// <summary>
        /// Retrieves a view repository for managing entities of type <typeparamref name="TEntity"/> >.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The repository.</returns>
        public IGenericReadRepository<TEntity> ViewRepository<TEntity>()
            where TEntity : Entity
        {
            var key = typeof(TEntity).Name;

            return (IGenericReadRepository<TEntity>)_viewRepositories.GetOrAdd(key, _ =>
            {
                return ActivatorUtilities.CreateInstance<GenericViewRepository<TEntity>>(_serviceProvider, _context);
            });
        }

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _ = await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Commits the current transaction asynchronously.
        /// If an error occurs during the commit, the transaction is rolled back.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(_transaction);
            await SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }

        /// <summary>
        /// Rolls back the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(_transaction);

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clears the repositories and resets the change tracker in the context.
        /// </summary>
        public void ResetRepositories()
        {
            _repositories.Clear();
            _context.ChangeTracker.Clear();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }

}
