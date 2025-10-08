using Microsoft.EntityFrameworkCore.Query;
using Stickto.Shared.Abstractions.Entities;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Stickto.Shared.Infrastructure.Specifications
{
    /// <summary>
    /// Provides a base class for specifications that can be used to filter, order,
    /// and group entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the output result.</typeparam>
    public abstract class Specification<TEntity, TResult> : ISpecification<TEntity, TResult>
                    where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{TEntity, TResult}"/> class.
        /// </summary>
        protected Specification()
        {
        }

        /// <summary>
        /// Gets the predicates used to filter the entities.
        /// </summary>
        public Collection<Expression<Func<TEntity, bool>>> Predicates { get; } = [];

        /// <summary>
        /// Gets the includes used to include related entities.
        /// </summary>
        public Collection<Expression<Func<TEntity, object>>> IncludesParams { get; } = [];

        /// <summary>
        /// Gets the functions used to include related entities.
        /// </summary>
        public Collection<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> IncludeFunctions { get; } = [];

        /// <summary>
        /// Gets the expression used to order the entities.
        /// </summary>
        public Expression<Func<TEntity, object>> OrderBy { get; private set; }

        /// <summary>
        /// Gets the expression used to order the entities in descending order.
        /// </summary>
        public Expression<Func<TEntity, object>> OrderByDescending { get; private set; }

        /// <summary>
        /// Gets the page size for paging.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the page number for paging.
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Gets a value indicating whether paging is enabled.
        /// </summary>
        public bool IsPagingEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether tracking is disabled.
        /// </summary>
        public bool DisableTracking { get; private set; }

        /// <summary>
        /// Gets a value indicating whether query spliting is enabled.
        /// </summary>
        public bool EnableSplitQuery { get; private set; }

        /// <summary>
        /// Gets the expression used to select a subset of properties from the entities.
        /// This property is used when you want to select specific fields from the entity.
        /// </summary>
        public Expression<Func<TEntity, TResult>> SelectPredicate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the entities should be distinct.
        /// </summary>
        public bool IsDistinct { get; private set; }

        /// <summary>
        /// Adds a predicate to the specification.
        /// </summary>
        /// <param name="criteria">The predicate to add.</param>
        protected void AddPredicate(Expression<Func<TEntity, bool>> criteria)
        {
            Predicates.Add(criteria);
        }

        /// <summary>
        /// Adds an include to the specification.
        /// </summary>
        /// <param name="includeExpression">The include to add.</param>
        protected virtual void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            IncludesParams.Add(includeExpression);
        }

        /// <summary>
        /// Adds an include function to the specification.
        /// </summary>
        /// <param name="includeFunction">The include function to add.</param>
        protected void AddInclude(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>
            includeFunction)
        {
            IncludeFunctions.Add(includeFunction);
        }

        /// <summary>
        /// Disables tracking for the specification.
        /// </summary>
        protected void ApplyDisableTracking()
        {
            DisableTracking = true;
        }

        /// <summary>
        /// Enable query spliting for the specification.
        /// </summary>
        protected void ApplyQuerySplitting()
        {
            EnableSplitQuery = true;
        }

        /// <summary>
        /// Applies paging to the specification.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        protected void ApplyPaging(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            IsPagingEnabled = true;
        }

        /// <summary>
        /// Applies ordering to the specification.
        /// </summary>
        /// <param name="orderByExpression">The expression to order by.</param>
        protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        /// <summary>
        /// Applies descending ordering to the specification.
        /// </summary>
        /// <param name="orderByDescendingExpression">The expression to order by descending.</param>
        protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        /// <summary>
        /// Adds a select expression to the specification. This is used to select a subset of properties from the entities.
        /// </summary>
        /// <param name="selectPredicate">The select expression to add.</param>
        protected void AddSelect(Expression<Func<TEntity, TResult>> selectPredicate)
        {
            SelectPredicate = selectPredicate;
        }

        /// <summary>
        /// Applies distinct to the specification.
        /// </summary>
        protected void ApplyDistinct()
        {
            IsDistinct = true;
        }
    }

}
