namespace DTOs.Common;

/// <summary>
/// Generic wrapper for paginated query results.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// The items on the current page.
    /// </summary>
    public List<T> Data { get; set; } = [];

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page index (1-based).
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Whether a previous page exists.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Whether a next page exists.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;
}
