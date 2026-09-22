using System.ComponentModel;

namespace DTOs.Common;

/// <summary>
/// Paging arguments accepted by the listing endpoints.
/// </summary>
/// <remarks>
/// Values outside the supported range are clamped rather than rejected, so a client
/// that asks for page 0 or for ten thousand rows gets a sensible page instead of an error.
/// </remarks>
public class PageRequest
{
    /// <summary>
    /// Largest page size a caller may ask for.
    /// </summary>
    public const int MaxPageSize = 100;

    private const int DefaultPageSize = 10;

    private readonly int page = 1;
    private readonly int pageSize = DefaultPageSize;

    /// <summary>
    /// The page number (1-based). Defaults to 1.
    /// </summary>
    [DefaultValue(1)]
    public int Page
    {
        get => page;
        init => page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// The number of items per page. Defaults to 10, capped at <see cref="MaxPageSize"/>.
    /// </summary>
    [DefaultValue(DefaultPageSize)]
    public int PageSize
    {
        get => pageSize;
        init => pageSize = value switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }
}
