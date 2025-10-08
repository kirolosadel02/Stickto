using Stickto.Shared.Abstractions.Entities;
using Stickto.Shared.Abstractions.Responses;
using Stickto.Shared.Infrastructure.Specifications;
using System.Linq.Expressions;

namespace Stickto.Shared.Infrastructure.Repositories
{
    public interface IGenericReadRepository<TEntity>
    where TEntity : Entity
    {
        /// <summary>
        /// Asynchronously retrieves entities that satisfy the specified specification.
        /// </summary>
        /// <param name="specification">The specification to which the entities should conform.
        /// If null, all entities are retrieved.</param>
        /// <param name="cancellationToken">A token that can be used to request
        /// cancellation of the asynchronous operation.</param>
        /// <typeparam name="TResult">The type of the result that the specification produces.
        /// This is typically used to shape the data returned from the query,
        /// such as selecting only certain properties of the entity,
        /// performing calculations, etc.</typeparam>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the entities that satisfy the specification.</returns>
        Task<IEnumerable<TResult>> Find<TResult>(ISpecification<TEntity, TResult>? specification = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves paginated entities that satisfy the specified specification.
        /// </summary>
        /// <param name="specification">The specification to which the entities should conform.
        /// If null, all entities are retrieved.</param>
        /// <param name="cancellationToken">A token that can be used to request
        /// cancellation of the asynchronous operation.</param>
        /// <typeparam name="TResult">The type of the result that the specification produces.
        /// This is typically used to shape the data returned from the query,
        /// such as selecting only certain properties of the entity, performing calculations, etc.</typeparam>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the paginated entities that satisfy the specification.</returns>
        Task<PagedList<TResult>> FindPagination<TResult>(
                        ISpecification<TEntity, TResult>? specification = null,
                        CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously determines whether the repository contains any entities that satisfy the specified specification.
        /// </summary>
        /// <param name="specification">The specification to which the entities should conform.
        /// If null, the method checks if there are any entities at all.</param>
        /// <param name="cancellationToken">A token that can be used to request
        /// cancellation of the asynchronous operation.</param>
        /// <typeparam name="TResult">The type of the result that the specification produces.
        /// This is typically used to shape the data returned from the query,
        /// such as selecting only certain properties of the entity, performing calculations, etc.</typeparam>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is true if any entities satisfy the specification, false otherwise.</returns>
        Task<bool> Contains<TResult>(ISpecification<TEntity, TResult>? specification = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously determines whether the repository contains
        /// any entities that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to which the entities should conform.</param>
        /// <param name="cancellationToken">A token that can be used to request
        /// cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result indicates whether any entities satisfy the predicate.</returns>
        Task<bool> Contains(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously counts the number of entities that satisfy the specified specification.
        /// </summary>
        /// <param name="specification">The specification to which the entities should conform.
        /// If null, the method counts all entities.</param>
        /// <param name="cancellationToken">A token that can be used to request
        /// cancellation of the asynchronous operation.</param>
        /// <typeparam name="TResult">The type of the result that the specification produces.
        /// This is typically used to shape the data returned from the query,
        /// such as selecting only certain properties of the entity, performing calculations, etc.</typeparam>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the number of entities that satisfy the specification.</returns>
        Task<int> Count<TResult>(ISpecification<TEntity, TResult>? specification = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously counts the number of entities that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to which the entities should conform.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of entities.</returns>
        Task<int> Count(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves grouped data that satisfies the specified grouping specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result that the grouping specification produces.</typeparam>
        /// <param name="specification">The grouping specification to apply. Cannot be null.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the grouped data that satisfies the specification.</returns>
        Task<IEnumerable<TResult>> FindGrouped<TKey, TResult>(
            IGroupingSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class;

        /// <summary>
        /// Asynchronously retrieves paginated grouped data that satisfies the specified grouping specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result that the grouping specification produces.</typeparam>
        /// <param name="specification">The grouping specification to apply. Cannot be null.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the paginated grouped data that satisfies the specification.</returns>
        Task<PagedList<TResult>> FindGroupedPagination<TKey, TResult>(
            IGroupingSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class;

        /// <summary>
        /// Asynchronously retrieves paginated grouped data with aggregate values that satisfies the specified aggregating specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result that the grouping specification produces.</typeparam>
        /// <typeparam name="TAggregate">The type containing aggregate values computed from the entire dataset.</typeparam>
        /// <param name="specification">The aggregating specification to apply. Cannot be null.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the paginated grouped data along with aggregate values that satisfy the specification.</returns>
        Task<AggregatedPagedList<TResult, TAggregate>> FindGroupedAggregatedPagination<TKey, TResult, TAggregate>(
            IAggregatingSpecification<TEntity, TKey, TResult, TAggregate> specification,
            CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class
            where TAggregate : class;

        /// <summary>
        /// Asynchronously counts the number of groups that satisfy the specified grouping specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result that the grouping specification produces.</typeparam>
        /// <param name="specification">The grouping specification to apply. Cannot be null.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the number of groups that satisfy the specification.</returns>
        Task<int> CountGroups<TKey, TResult>(
            IGroupingSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken = default)
            where TKey : notnull
            where TResult : class;

        /// <summary>
        /// Applies the specified specification to the repository and returns the resulting queryable collection of entities.
        /// </summary>
        /// <typeparam name="TResult">The type of the result that the specification produces.
        /// This is typically used to shape the data returned from the query,
        /// such as selecting only certain properties of the entity, performing calculations, etc.</typeparam>
        /// <param name="spec">The specification to apply to the repository.
        /// If null, the method returns all entities as a queryable collection.</param>
        /// <returns>A queryable collection of entities that satisfy the specification.</returns>
        IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult>? spec);

        IQueryable<TResult> ApplyGroupingSpecification<TKey, TResult>(IGroupingSpecification<TEntity, TKey, TResult> specification)
            where TKey : notnull
            where TResult : class;
    }

}
