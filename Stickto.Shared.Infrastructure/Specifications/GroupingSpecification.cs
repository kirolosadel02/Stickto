using Stickto.Shared.Abstractions.Entities;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Stickto.Shared.Infrastructure.Specifications
{
    /// <summary>
    /// Provides a base class for specifications that support grouping operations
    /// on entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
    /// <typeparam name="TResult">The type of the output result.</typeparam>
    public class GroupingSpecification<TEntity, TKey, TResult>
        : Specification<TEntity, TResult>, IGroupingSpecification<TEntity, TKey, TResult>
        where TEntity : Entity
        where TKey : notnull
        where TResult : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingSpecification{TEntity, TKey, TResult}"/> class.
        /// </summary>
        protected GroupingSpecification()
        {
        }

        /// <summary>
        /// Gets the expression used to group the entities.
        /// </summary>
        public Expression<Func<TEntity, TKey>> GroupBy { get; private set; }

        /// <summary>
        /// Gets the expression used to order the groups.
        /// </summary>
        public Expression<Func<IGrouping<TKey, TEntity>, object>> OrderByGroups { get; private set; }

        /// <summary>
        /// Gets the expression used to order the groups in descending order.
        /// </summary>
        public Expression<Func<IGrouping<TKey, TEntity>, object>> OrderByGroupsDescending { get; private set; }

        /// <summary>
        /// Gets the expression used to select the final result from the groups.
        /// </summary>
        public Expression<Func<IGrouping<TKey, TEntity>, TResult>> GroupSelection { get; private set; }

        /// <summary>
        /// Gets the collection of predicates to filter groups (HAVING clause equivalent).
        /// </summary>
        public Collection<Expression<Func<IGrouping<TKey, TEntity>, bool>>> HavingPredicates { get; } = [];

        /// <summary>
        /// Gets the collection of predicates to filter the results after grouping and selection.
        /// These are applied to the TResult objects.
        /// </summary>
        public Collection<Expression<Func<TResult, bool>>> PostSelectionPredicates { get; } = [];

        /// <summary>
        /// Gets the expression used to order the results after selection.
        /// </summary>
        public Expression<Func<TResult, object>> PostSelectionOrderBy { get; private set; }

        /// <summary>
        /// Gets the expression used to order the results after selection in descending order.
        /// </summary>
        public Expression<Func<TResult, object>> PostSelectionOrderByDescending { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the results should be distinct after selection.
        /// </summary>
        public bool IsPostSelectionDistinct { get; private set; }

        /// <summary>
        /// Adds a predicate to filter groups after grouping (equivalent to SQL HAVING clause).
        /// </summary>
        /// <param name="havingPredicate">The predicate to filter groups.</param>
        protected virtual void AddHavingPredicate(Expression<Func<IGrouping<TKey, TEntity>, bool>> havingPredicate)
        {
            HavingPredicates.Add(havingPredicate);
        }

        /// <summary>
        /// Adds a predicate to filter results after grouping and selection.
        /// </summary>
        /// <param name="postSelectionPredicate">The predicate to filter results.</param>
        protected virtual void AddPostSelectionPredicate(Expression<Func<TResult, bool>> postSelectionPredicate)
        {
            PostSelectionPredicates.Add(postSelectionPredicate);
        }

        /// <summary>
        /// Applies grouping to the specification.
        /// </summary>
        /// <param name="groupByExpression">The expression to group by.</param>
        protected virtual void ApplyGroupBy(Expression<Func<TEntity, TKey>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        /// <summary>
        /// Applies ordering to the groups.
        /// </summary>
        /// <param name="orderByGroupsExpression">The expression to order groups by.</param>
        protected virtual void ApplyOrderByGroups(Expression<Func<IGrouping<TKey, TEntity>, object>> orderByGroupsExpression)
        {
            OrderByGroups = orderByGroupsExpression;
        }

        /// <summary>
        /// Applies descending ordering to the groups.
        /// </summary>
        /// <param name="orderByGroupsDescendingExpression">The expression to order groups by descending.</param>
        protected virtual void ApplyOrderByGroupsDescending(Expression<Func<IGrouping<TKey, TEntity>, object>> orderByGroupsDescendingExpression)
        {
            OrderByGroupsDescending = orderByGroupsDescendingExpression;
        }

        /// <summary>
        /// Applies ordering to the results after selection.
        /// </summary>
        /// <param name="postSelectionOrderByExpression">The expression to order results by.</param>
        protected virtual void ApplyPostSelectionOrderBy(Expression<Func<TResult, object>> postSelectionOrderByExpression)
        {
            PostSelectionOrderBy = postSelectionOrderByExpression;
        }

        /// <summary>
        /// Applies descending ordering to the results after selection.
        /// </summary>
        /// <param name="postSelectionOrderByDescendingExpression">The expression to order results by descending.</param>
        protected virtual void ApplyPostSelectionOrderByDescending(Expression<Func<TResult, object>> postSelectionOrderByDescendingExpression)
        {
            PostSelectionOrderByDescending = postSelectionOrderByDescendingExpression;
        }

        /// <summary>
        /// Sets the selection expression for transforming grouped entities into the result type.
        /// </summary>
        /// <param name="groupSelection">The selection expression for grouped entities.</param>
        protected virtual void SetGroupSelection(Expression<Func<IGrouping<TKey, TEntity>, TResult>> groupSelection)
        {
            GroupSelection = groupSelection;
        }

        /// <summary>
        /// Applies distinct to the results after selection.
        /// </summary>
        protected virtual void ApplyPostSelectionDistinct()
        {
            IsPostSelectionDistinct = true;
        }
    }

}
