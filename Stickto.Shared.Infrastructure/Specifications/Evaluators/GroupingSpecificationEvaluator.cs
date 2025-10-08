using Microsoft.EntityFrameworkCore;
using Stickto.Shared.Abstractions.Entities;
using Stickto.Shared.Abstractions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stickto.Shared.Infrastructure.Specifications.Evaluators
{
    /// <summary>
    /// Provides a mechanism for constructing a query based on a grouping specification
    /// for entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public static class GroupingSpecificationEvaluator<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Constructs a query based on the provided grouping specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="inputQuery">The initial query.</param>
        /// <param name="specification">The grouping specification to apply.</param>
        /// <returns>A query that includes all the specifications.</returns>
        public static IQueryable<TResult> GetQuery<TKey, TResult>(
            IQueryable<TEntity> inputQuery,
            IGroupingSpecification<TEntity, TKey, TResult> specification)
            where TKey : notnull
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            // Get base grouped query
            var groupedQuery = GetBaseGroupedQuery(inputQuery, specification);

            // Apply selection expression to transform from groups to result
            if (specification.GroupSelection == null)
            {
                throw new InvalidOperationException("A GroupSelection must be specified to transform grouped results.");
            }

            // Apply selection and post-selection operations
            return ApplyPostSelectionOperations(
                groupedQuery.Select(specification.GroupSelection),
                specification);
        }

        /// <summary>
        /// Constructs a paginated query based on the provided grouping specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="inputQuery">The initial query.</param>
        /// <param name="specification">The grouping specification to apply.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A paginated list that includes all the specifications.</returns>
        public static async Task<PagedList<TResult>> GetPaginatedQuery<TKey, TResult>(
            IQueryable<TEntity> inputQuery,
            IGroupingSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken)
            where TKey : notnull
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(specification);

            // Get the base query without paging
            var query = GetQuery(inputQuery, specification);

            // Get paginated items
            var (items, totalCount) = await GetPaginatedItems(query, specification, cancellationToken);

            return new PagedList<TResult>(
                items,
                totalCount,
                specification.PageNumber,
                specification.PageSize);
        }

        /// <summary>
        /// Constructs a paginated query with aggregate values based on the provided specification.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TAggregate">The type containing aggregate values.</typeparam>
        /// <param name="inputQuery">The initial query.</param>
        /// <param name="specification">The aggregating specification to apply.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A paginated list with aggregate values that includes all the specifications.</returns>
        public static async Task<AggregatedPagedList<TResult, TAggregate>> GetAggregatedPaginatedQuery<TKey, TResult, TAggregate>(
            IQueryable<TEntity> inputQuery,
            IAggregatingSpecification<TEntity, TKey, TResult, TAggregate> specification,
            CancellationToken cancellationToken)
            where TKey : notnull
            where TResult : class
            where TAggregate : class
        {
            ArgumentNullException.ThrowIfNull(specification);
            ArgumentNullException.ThrowIfNull(specification.AggregateValuesSelector);

            // Get the base grouped query before selection
            var baseGroupedQuery = GetBaseGroupedQuery(inputQuery, specification);

            // Ensure we have a selection expression
            if (specification.GroupSelection == null)
            {
                throw new InvalidOperationException("A GroupSelection must be specified to transform grouped results.");
            }

            // Apply selection and post-selection operations
            var resultQuery = ApplyPostSelectionOperations(
                baseGroupedQuery.Select(specification.GroupSelection),
                specification);

            // Compute aggregate values from the base grouped query before pagination
            var aggregateValues = specification.AggregateValuesSelector(resultQuery);

            // Get paginated items
            var (items, totalCount) = await GetPaginatedItems(resultQuery, specification, cancellationToken);

            return new AggregatedPagedList<TResult, TAggregate>(
                items,
                totalCount,
                specification.PageNumber,
                specification.PageSize,
                aggregateValues);
        }

        /// <summary>
        /// Gets the base grouped query before selection and pagination.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        private static IQueryable<IGrouping<TKey, TEntity>> GetBaseGroupedQuery<TKey, TResult>(
            IQueryable<TEntity> inputQuery,
            IGroupingSpecification<TEntity, TKey, TResult> specification)
            where TKey : notnull
            where TResult : class
        {
            // Initial query configuration
            IQueryable<TEntity> query = inputQuery.AsQueryable();

            if (specification.DisableTracking)
            {
                query = inputQuery.AsNoTracking();
            }

            if (specification.EnableSplitQuery)
            {
                query = query.AsSplitQuery();
            }

            // Apply includes
            query = specification.IncludesParams.Aggregate(
                query, (current, include) => current.Include(include));

            if (specification.IncludeFunctions.Count > 0)
            {
                query = specification.IncludeFunctions.Aggregate(
                    query, (current, include) => include(current));
            }

            // Apply pre-grouping filters
            query = specification.Predicates.Aggregate(
                query, (current, predicate) => current.Where(predicate));

            // Apply distinct if required (before grouping)
            if (specification.IsDistinct)
            {
                query = query.Distinct();
            }

            // Apply pre-grouping ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Ensure we have a grouping expression
            if (specification.GroupBy == null)
            {
                throw new InvalidOperationException("A GroupBy expression must be specified for a grouping specification.");
            }

            // Apply grouping
            var groupedQuery = query.GroupBy(specification.GroupBy);

            // Apply HAVING clause filters (post-grouping filters)
            groupedQuery = specification.HavingPredicates.Aggregate(
                groupedQuery, (current, predicate) => current.Where(predicate));

            // Apply post-grouping ordering on the groups themselves
            if (specification.OrderByGroups != null)
            {
                groupedQuery = groupedQuery.OrderBy(specification.OrderByGroups);
            }
            else if (specification.OrderByGroupsDescending != null)
            {
                groupedQuery = groupedQuery.OrderByDescending(specification.OrderByGroupsDescending);
            }

            return groupedQuery;
        }

        /// <summary>
        /// Applies post-selection filters and sorting operations.
        /// </summary>
        private static IQueryable<TResult> ApplyPostSelectionOperations<TKey, TResult>(
            IQueryable<TResult> query,
            IGroupingSpecification<TEntity, TKey, TResult> specification)
            where TKey : notnull
            where TResult : class
        {
            // Apply post-selection filters
            query = specification.PostSelectionPredicates.Aggregate(
                query, (current, predicate) => current.Where(predicate));

            // Apply post-selection distinct if required
            if (specification.IsPostSelectionDistinct)
            {
                query = query.Distinct();
            }

            // Apply post-selection ordering
            if (specification.PostSelectionOrderBy != null)
            {
                query = query.OrderBy(specification.PostSelectionOrderBy);
            }
            else if (specification.PostSelectionOrderByDescending != null)
            {
                query = query.OrderByDescending(specification.PostSelectionOrderByDescending);
            }

            return query;
        }

        /// <summary>
        /// Gets the paginated items from a query and also returns the total count.
        /// </summary>
        private static async Task<(List<TResult> Items, int TotalCount)> GetPaginatedItems<TKey, TResult>(
            IQueryable<TResult> query,
            IGroupingSpecification<TEntity, TKey, TResult> specification,
            CancellationToken cancellationToken)
            where TKey : notnull
            where TResult : class
        {
            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                // Get the total count
                int totalCount = await query.CountAsync(cancellationToken);

                // Calculate pagination values
                var totalPages = (int)Math.Ceiling((double)totalCount / specification.PageSize);
                int pageNumber = specification.PageNumber > totalPages ? totalPages : specification.PageNumber;
                pageNumber = pageNumber < 1 ? 1 : pageNumber; // Ensure page number is at least 1

                // Apply skip/take paging
                var items = await query
                    .Skip((pageNumber - 1) * specification.PageSize)
                    .Take(specification.PageSize)
                    .ToListAsync(cancellationToken);

                return (items, totalCount);
            }
            else
            {
                // If paging isn't enabled, get all items and count
                var items = await query.ToListAsync(cancellationToken);
                return (items, items.Count);
            }
        }
    }

}
