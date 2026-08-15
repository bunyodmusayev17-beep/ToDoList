using System.ComponentModel.DataAnnotations;
using ToDoList.Domain.Entities;

namespace ToDoList.BlazorUI.Models;

public class ToDoItemDto
{
    public long ToDoItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsDeleted { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? ReminderAt { get; set; }

    public bool IsOverdue => !IsCompleted && DueDate.HasValue && DueDate.Value.Date < DateTime.UtcNow.Date;
}

public class ToDoItemCreateRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
    public string? Description { get; set; }

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderAt { get; set; }
}

public class ToDoItemUpdateRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
    public string? Description { get; set; }

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderAt { get; set; }
}
