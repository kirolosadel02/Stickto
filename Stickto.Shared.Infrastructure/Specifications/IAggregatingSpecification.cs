using Stickto.Shared.Abstractions.Entities;

namespace Stickto.Shared.Infrastructure.Specifications
{
    /// <summary>
    /// Defines a contract for a specification that supports grouping operations with aggregate values
    /// on entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key used for grouping.</typeparam>
    /// <typeparam name="TResult">The type of the output result.</typeparam>
    /// <typeparam name="TAggregate">The type containing aggregate values.</typeparam>
    public interface IAggregatingSpecification<TEntity, TKey, TResult, TAggregate>
        : IGroupingSpecification<TEntity, TKey, TResult>
        where TEntity : Entity
        where TKey : notnull
        where TResult : class
        where TAggregate : class
    {
        /// <summary>
        /// Gets the function to compute aggregate values from the results after group selection but before applying pagination.
        /// </summary>
        Func<IQueryable<TResult>, TAggregate> AggregateValuesSelector { get; }
    }
}
