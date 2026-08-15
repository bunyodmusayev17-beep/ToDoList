using System.ComponentModel.DataAnnotations;
using ToDoList.Domain.Entities;

namespace ToDoList.BlazorUI.Models;

public class TaskFormModel
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
    public string? Description { get; set; }

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderAt { get; set; }
    public bool IsCompleted { get; set; }

    public ToDoItemCreateRequest ToCreateRequest() => new()
    {
        Title = Title,
        Description = Description,
        Priority = Priority,
        DueDate = DueDate,
        ReminderAt = ReminderAt
    };

    public ToDoItemUpdateRequest ToUpdateRequest() => new()
    {
        Title = Title,
        Description = Description,
        Priority = Priority,
        IsCompleted = IsCompleted,
        DueDate = DueDate,
        ReminderAt = ReminderAt
    };

    public static TaskFormModel FromDto(ToDoItemDto dto) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        Priority = dto.Priority,
        DueDate = dto.DueDate,
        ReminderAt = dto.ReminderAt,
        IsCompleted = dto.IsCompleted
    };
}
