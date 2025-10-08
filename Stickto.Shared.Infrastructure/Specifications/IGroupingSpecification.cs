using Stickto.Shared.Abstractions.Entities;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Stickto.Shared.Infrastructure.Specifications
{
    /// <summary>
    /// Defines a contract for a specification that supports grouping operations
    /// on entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
    /// <typeparam name="TResult">The type of the output result.</typeparam>
    public interface IGroupingSpecification<TEntity, TKey, TResult>
        : ISpecification<TEntity, TResult>
        where TEntity : Entity
        where TKey : notnull
        where TResult : class
    {
        /// <summary>
        /// Gets the expression used to group the entities.
        /// </summary>
        Expression<Func<TEntity, TKey>> GroupBy { get; }

        /// <summary>
        /// Gets the expression used to order the groups.
        /// </summary>
        Expression<Func<IGrouping<TKey, TEntity>, object>> OrderByGroups { get; }

        /// <summary>
        /// Gets the expression used to order the groups in descending order.
        /// </summary>
        Expression<Func<IGrouping<TKey, TEntity>, object>> OrderByGroupsDescending { get; }

        /// <summary>
        /// Gets the expression used to select the final result from the groups.
        /// </summary>
        Expression<Func<IGrouping<TKey, TEntity>, TResult>> GroupSelection { get; }

        /// <summary>
        /// Gets the collection of predicates to filter groups (HAVING clause equivalent).
        /// </summary>
        Collection<Expression<Func<IGrouping<TKey, TEntity>, bool>>> HavingPredicates { get; }

        /// <summary>
        /// Gets the collection of predicates to filter the results after grouping and selection.
        /// These are applied to the TResult objects.
        /// </summary>
        Collection<Expression<Func<TResult, bool>>> PostSelectionPredicates { get; }

        /// <summary>
        /// Gets the expression used to order the results after selection.
        /// </summary>
        Expression<Func<TResult, object>> PostSelectionOrderBy { get; }

        /// <summary>
        /// Gets the expression used to order the results after selection in descending order.
        /// </summary>
        Expression<Func<TResult, object>> PostSelectionOrderByDescending { get; }

        /// <summary>
        /// Gets a value indicating whether the results should be distinct after selection.
        /// </summary>
        bool IsPostSelectionDistinct { get; }
    }

}
