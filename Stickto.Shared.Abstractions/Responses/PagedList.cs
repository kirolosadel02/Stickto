namespace Stickto.Shared.Abstractions.Responses
{
    /// <summary>
    /// Represents a paginated list of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <param name="totalCount">The total number of items.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        public PagedList(ICollection<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        }

        /// <summary>
        /// Gets an empty <see cref="PagedList{T}"/>.
        /// </summary>
        public static PagedList<T> Empty => new(Array.Empty<T>(), 0, 0, 0);

        /// <summary>
        /// Gets the items in the current page.
        /// </summary>
        public ICollection<T> Items { get; }

        /// <summary>
        /// Gets the total number of items.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the number of total pages.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// Gets the number of items per page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => CurrentPage * PageSize < TotalCount;

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;
    }

}
