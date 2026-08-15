using ToDoList.Domain.Entities;

namespace ToDoList.Application.Dtos;

public enum ToDoItemSortBy
{
    CreatedAt = 0,
    DueDate = 1,
    Priority = 2,
    Title = 3
}

public class ToDoItemQueryParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    private int _page = 1;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : (value > MaxPageSize ? MaxPageSize : value);
    }

    public bool? IsCompleted { get; set; }
    public PriorityLevel? Priority { get; set; }
    public DateTime? DueFrom { get; set; }
    public DateTime? DueTo { get; set; }
    public ToDoItemSortBy SortBy { get; set; } = ToDoItemSortBy.CreatedAt;
    public bool SortDescending { get; set; } = true;
}
