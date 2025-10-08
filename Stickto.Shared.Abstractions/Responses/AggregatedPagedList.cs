namespace Stickto.Shared.Abstractions.Responses
{
    /// <summary>
    /// Represents a paginated list of items with aggregate values computed from the entire dataset.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    /// <typeparam name="TAggregate">The type containing aggregate values.</typeparam>
    public sealed class AggregatedPagedList<T, TAggregate> : PagedList<T>
        where TAggregate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedPagedList{T, TAggregate}"/> class.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <param name="totalCount">The total number of items.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="aggregateValues">The aggregate values computed from the entire dataset.</param>
        public AggregatedPagedList(
            ICollection<T> items,
            int totalCount,
            int page,
            int pageSize,
            TAggregate? aggregateValues)
            : base(items, totalCount, page, pageSize)
        {
            AggregateValues = aggregateValues;
        }

        /// <summary>
        /// Gets the aggregate values computed from the entire dataset.
        /// </summary>
        public TAggregate? AggregateValues { get; }

        /// <summary>
        /// Gets an empty <see cref="AggregatedPagedList{T, TAggregate}"/> with default aggregate values.
        /// </summary>
        /// <param name="aggregateValues">The default aggregate values.</param>
        /// <returns>An empty <see cref="AggregatedPagedList{T, TAggregate}"/> with the specified aggregate values.</returns>
        public static new AggregatedPagedList<T, TAggregate> Empty(TAggregate aggregateValues) =>
            new(Array.Empty<T>(), 0, 0, 0, aggregateValues);
    }

}
