using Microsoft.EntityFrameworkCore;
using Stickto.Shared.Abstractions.Entities;
using Stickto.Shared.Abstractions.Responses;

namespace Stickto.Shared.Infrastructure.Specifications.Evaluators
{
    /// <summary>
    /// Provides a mechanism for constructing a query based on a specification
    /// for entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public static class SpecificationEvaluator<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Constructs a query based on the provided specification.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="inputQuery">The initial query.</param>
        /// <param name="specification">The specification to apply.</param>
        /// <returns>A query that includes all the specifications.</returns>
        public static IQueryable<TResult> GetQuery<TResult>(
            IQueryable<TEntity> inputQuery,
            ISpecification<TEntity, TResult> specification)
        {
            IQueryable<TResult> result;
            IQueryable<TEntity> query = inputQuery.AsQueryable();
            if (specification.DisableTracking)
            {
                query = inputQuery.AsQueryable().AsNoTracking();
            }

            if (specification.EnableSplitQuery)
            {
                query = query.AsSplitQuery();
            }

            // Includes all expression-based includes
            query = specification.IncludesParams.Aggregate(
                query, (current, include) => current.Include(include));

            if (specification.IncludeFunctions.Count > 0)
            {
                query = specification.IncludeFunctions.Aggregate(
                    query, (current, include) => include(current));
            }

            // modify the IQueryable using the specif	ication's criteria expression
            query = specification.Predicates.Aggregate(
                query, (current, include) => current.Where(include));

            // Apply distinct if required
            if (specification.IsDistinct)
            {
                query = query.Distinct();
            }

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            //Apply select predicate if expressions are set
            if (specification.SelectPredicate != null)
            {
                result = query.Select(specification.SelectPredicate);
            }
            else
            {
                result = query.Cast<TResult>();
            }

            //string queryString = result.ToQueryString();
            //Console.Write(queryString);

            return result;
        }

        /// <summary>
        /// Constructs a paginated query based on the provided specification.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="inputQuery">The initial query.</param>
        /// <param name="specification">The specification to apply.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A paginated list that includes all the specifications.</returns>
        public static async Task<PagedList<TResult>> GetPaginatedQuery<TResult>(
            IQueryable<TEntity> inputQuery,
            ISpecification<TEntity, TResult> specification,
            CancellationToken cancellationToken)
        {
            IQueryable<TResult> result;
            IQueryable<TEntity> query = inputQuery.AsQueryable();

            if (specification.DisableTracking)
            {
                query = inputQuery.AsQueryable().AsNoTracking();
            }

            if (specification.EnableSplitQuery)
            {
                query = query.AsSplitQuery();
            }

            // Includes all expression-based includes
            query = specification.IncludesParams.Aggregate(
                query, (current, include) => current.Include(include));

            if (specification.IncludeFunctions.Count > 0)
            {
                query = specification.IncludeFunctions.Aggregate(
                    query, (current, include) => include(current));
            }

            // modify the IQueryable using the specification's criteria expression
            query = specification.Predicates.Aggregate(
                query, (current, include) => current.Where(include));

            // Apply distinct if required
            if (specification.IsDistinct)
            {
                query = query.Distinct();
            }

            // Get the total count

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            int totalCount = 0;

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                // Get the total count
                totalCount = await query.CountAsync();
                if (totalCount != 0)
                {
                    var totalPages = (int)Math.Ceiling((double)totalCount / specification.PageSize);
                    int pageNumber = specification.PageNumber;

                    if (specification.PageNumber > totalPages)
                    {
                        pageNumber = totalPages;
                    }

                    if (pageNumber == 1)
                    {
                        query = query.Take(specification.PageSize);
                    }
                    else
                    {
                        var idProperty = GetIdPropertyName();
                        int skipItemsCount = ((pageNumber - 1) * specification.PageSize) - 1;

                        var lastIdOnPreviousPage = await query.OrderByDescending(e => EF.Property<object>(e, idProperty))
                            .Skip(skipItemsCount)
                            .Take(1)
                            .Select(e => EF.Property<object>(e, idProperty))
                            .FirstOrDefaultAsync();

                        if (lastIdOnPreviousPage != null)
                        {
                            query = query.Where(e => Comparer<object>.Default.Compare(EF.Property<object>(e, idProperty), lastIdOnPreviousPage) > 0)
                                .OrderBy(e => EF.Property<object>(e, idProperty))
                                .Take(specification.PageSize);
                        }
                    }
                }
            }

            //Apply select predicate if expressions are set
            if (specification.SelectPredicate != null)
            {
                result = query.Select(specification.SelectPredicate);
            }
            else
            {
                result = query.Cast<TResult>();
            }

            List<TResult> items = await result.ToListAsync(cancellationToken);

            return new PagedList<TResult>(
                items,
                totalCount,
                specification.PageNumber,
                specification.PageSize);
        }

        private static string GetIdPropertyName()
        {
            var idProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have an 'Id' property.");
            return idProperty.Name;
        }
    }

}
