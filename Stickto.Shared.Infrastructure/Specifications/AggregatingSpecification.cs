using Stickto.Shared.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stickto.Shared.Infrastructure.Specifications
{
    /// <summary>
    /// Provides a base class for specifications that support grouping operations with aggregate values
    /// on entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
    /// <typeparam name="TResult">The type of the output result.</typeparam>
    /// <typeparam name="TAggregate">The type containing aggregate values.</typeparam>
    public class AggregatingSpecification<TEntity, TKey, TResult, TAggregate>
        : GroupingSpecification<TEntity, TKey, TResult>,
          IAggregatingSpecification<TEntity, TKey, TResult, TAggregate>
        where TEntity : Entity
        where TKey : notnull
        where TResult : class
        where TAggregate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatingSpecification{TEntity, TKey, TResult, TAggregate}"/> class.
        /// </summary>
        protected AggregatingSpecification()
        {
        }

        /// <summary>
        /// Gets the function to compute aggregate values from the results after group selection but before applying pagination.
        /// </summary>
        public Func<IQueryable<TResult>, TAggregate> AggregateValuesSelector { get; private set; }

        /// <summary>
        /// Sets the function to compute aggregate values from the results after group selection.
        /// </summary>
        /// <param name="aggregateValuesSelector">The function to compute aggregate values.</param>
        protected virtual void SetAggregateValuesSelector(Func<IQueryable<TResult>, TAggregate> aggregateValuesSelector)
        {
            AggregateValuesSelector = aggregateValuesSelector ?? throw new ArgumentNullException(nameof(aggregateValuesSelector));
        }
    }

}
