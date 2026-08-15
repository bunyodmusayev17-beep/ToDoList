using System.Globalization;
using ToDoList.Domain.Entities;

namespace ToDoList.BlazorUI.Models;

public enum ToDoItemSortBy
{
    CreatedAt = 0,
    DueDate = 1,
    Priority = 2,
    Title = 3
}

public class ToDoItemQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsCompleted { get; set; }
    public PriorityLevel? Priority { get; set; }
    public DateTime? DueFrom { get; set; }
    public DateTime? DueTo { get; set; }
    public ToDoItemSortBy SortBy { get; set; } = ToDoItemSortBy.CreatedAt;
    public bool SortDescending { get; set; } = true;

    /// <summary>Builds the query-string for the GET /todoitems endpoint.</summary>
    public string ToQueryString()
    {
        var parts = new List<string>
        {
            $"page={Page}",
            $"pageSize={PageSize}",
            $"sortBy={SortBy}",
            $"sortDescending={SortDescending.ToString().ToLowerInvariant()}"
        };

        if (IsCompleted.HasValue)
        {
            parts.Add($"isCompleted={IsCompleted.Value.ToString().ToLowerInvariant()}");
        }

        if (Priority.HasValue)
        {
            parts.Add($"priority={Priority.Value}");
        }

        if (DueFrom.HasValue)
        {
            parts.Add($"dueFrom={Uri.EscapeDataString(DueFrom.Value.ToString("o", CultureInfo.InvariantCulture))}");
        }

        if (DueTo.HasValue)
        {
            parts.Add($"dueTo={Uri.EscapeDataString(DueTo.Value.ToString("o", CultureInfo.InvariantCulture))}");
        }

        return "?" + string.Join("&", parts);
    }
}
