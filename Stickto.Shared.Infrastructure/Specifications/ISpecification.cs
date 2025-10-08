using Microsoft.EntityFrameworkCore.Query;
using Stickto.Shared.Abstractions.Entities;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Stickto.Shared.Infrastructure.Specifications
{
    /// <summary>
    /// Defines a contract for a specification that can be used to filter, order, and group entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the output result.</typeparam>
    public interface ISpecification<TEntity, TResult>
                    where TEntity : Entity
    {
        /// <summary>
        /// Gets the predicates used to filter the entities.
        /// </summary>
        Collection<Expression<Func<TEntity, bool>>> Predicates { get; }

        /// <summary>
        /// Gets the includes used to include related entities.
        /// </summary>
        Collection<Expression<Func<TEntity, object>>> IncludesParams { get; }

        /// <summary>
        /// Gets the functions used to include related entities.
        /// </summary>
        Collection<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> IncludeFunctions { get; }

        /// <summary>
        /// Gets the expression used to order the entities.
        /// </summary>
        Expression<Func<TEntity, object>> OrderBy { get; }

        /// <summary>
        /// Gets the expression used to order the entities in descending order.
        /// </summary>
        Expression<Func<TEntity, object>> OrderByDescending { get; }

        /// <summary>
        /// Gets the expression used to select a subset of properties from the entities.
        /// </summary>
        Expression<Func<TEntity, TResult>> SelectPredicate { get; }

        /// <summary>
        /// Gets the page size for paging.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the page number for paging.
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Gets a value indicating whether paging is enabled.
        /// </summary>
        bool IsPagingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether tracking is disabled.
        /// </summary>
        bool DisableTracking { get; }

        /// <summary>
        /// Gets a value indicating whether query spliting is enabled.
        /// </summary>
        bool EnableSplitQuery { get; }

        /// <summary>
        /// Gets a value indicating whether the entities should be distinct.
        /// </summary>
        bool IsDistinct { get; }
    }

}
