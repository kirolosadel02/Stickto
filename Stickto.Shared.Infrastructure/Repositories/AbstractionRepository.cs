using Microsoft.EntityFrameworkCore;
using Stickto.Shared.Abstractions.Entities;
using Stickto.Shared.Abstractions.Responses;
using Stickto.Shared.Infrastructure.Specifications;
using Stickto.Shared.Infrastructure.Specifications.Evaluators;
using System.Linq.Expressions;

namespace Stickto.Shared.Infrastructure.Repositories
{
    /// <summary>
    /// The AbstractionRepository class is a generic repository that provides
    /// basic CRUD operations for any entity class.
    /// </summary>
    /// <typeparam name="T">The type of the entity class.</typeparam>
    public class AbstractionRepository<T> : IGenericReadRepository<T>
        where T : Entity
    {
        /// <summary>
        /// The DbSet representing the entity in the database.
        /// </summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// The context of the database.
        /// </summary>
        protected readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractionRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AbstractionRepository(ApplicationDbContext context)
        {
            this._dbSet = context.Set<T>();
            this._dbContext = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResult>> Find<TResult>(
            ISpecification<T, TResult>? specification = null,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult>? spec)
        {
            return spec is null ? _dbSet.AsQueryable().Cast<TResult>() :
                SpecificationEvaluator<T>
                .GetQuery(_dbSet, spec);
        }

        /// <inheritdoc/>
        public async Task<PagedList<TResult>> FindPagination<TResult>(
                        ISpecification<T, TResult>? specification = null,
                        CancellationToken cancellationToken = default)
        {
            return specification is null ? default : await SpecificationEvaluator<T>
                .GetPaginatedQuery(_dbSet, specification, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> Contains<TResult>(ISpecification<T, TResult>? specification = null, CancellationToken cancellationToken = default)
        {
            return await Count(specification, cancellationToken) > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> Contains(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await Count(predicate, cancellationToken) > 0;
        }

        /// <inheritdoc/>
        public async Task<int> Count<TResult>(ISpecification<T, TResult>? specification = null, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).CountAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> Count(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return _dbSet.Where(predicate).CountAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResult>> FindGrouped<TKey, TResult>(
        IGroupingSpecification<T, TKey, TResult> specification,
        CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            return await ApplyGroupingSpecification(specification).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<PagedList<TResult>> FindGroupedPagination<TKey, TResult>(
        IGroupingSpecification<T, TKey, TResult> specification,
        CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            return await GroupingSpecificationEvaluator<T>.GetPaginatedQuery(
                _dbSet,
                specification,
                cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<AggregatedPagedList<TResult, TAggregate>> FindGroupedAggregatedPagination<TKey, TResult, TAggregate>(
            IAggregatingSpecification<T, TKey, TResult, TAggregate> specification,
            CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class
            where TAggregate : class
        {
            return await GroupingSpecificationEvaluator<T>
                .GetAggregatedPaginatedQuery(_dbSet, specification, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> CountGroups<TKey, TResult>(
            IGroupingSpecification<T, TKey, TResult> specification,
            CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            return await ApplyGroupingSpecification(specification).CountAsync(cancellationToken);
        }

        public IQueryable<TResult> ApplyGroupingSpecification<TKey, TResult>(
        IGroupingSpecification<T, TKey, TResult> specification)
        where TKey : notnull
        where TResult : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            return GroupingSpecificationEvaluator<T>.GetQuery(
                _dbSet,
                specification);
        }

    }

}
