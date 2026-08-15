using ToDoList.Domain.Entities;

namespace ToDoList.Application.Dtos;

public class ToDoItemGetDto
{
    public long ToDoItemId { get; set; }
    public string Title { get; set; }
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
    public UserGetDto? UserGetDto { get; set; }
}
