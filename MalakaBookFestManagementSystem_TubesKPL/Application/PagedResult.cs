namespace MalakaBookFest.Application.Common;

/// <summary>
/// Generic pagination wrapper for list endpoints. Carries data alongside
/// metadata so clients can implement pagination controls without extra calls.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize) =>
        new() { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
}